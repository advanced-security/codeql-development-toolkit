#!/bin/bash

# End-to-end validation script for CodeQL Toolkit CLI
# This script validates the new-query subcommand functionality across all supported languages

set -euo pipefail

# Configuration - Centralized path variables
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
QLT_PROJECT_DIR="$REPO_ROOT/src/CodeQLToolkit.Core"
QLT_PROJECT_FILE="$QLT_PROJECT_DIR/CodeQLToolkit.Core.csproj"
QLT_BINARY="$QLT_PROJECT_DIR/bin/Debug/net6.0/CodeQLToolkit.Core.dll"
TEST_DIR="${TEST_DIR:-/tmp/qlt-cli-e2e-test}"
DEFAULT_LANGUAGES=("cpp" "csharp" "go" "java" "javascript" "python" "ruby")
DEFAULT_QUERY_KINDS=("problem" "path-problem")
LANGUAGES=("${DEFAULT_LANGUAGES[@]}")
QUERY_KINDS=("${DEFAULT_QUERY_KINDS[@]}")
RUN_TESTS=${RUN_TESTS:-true}  # Default to running tests

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Cleanup function
cleanup_at_start() {
    if [[ -d "$TEST_DIR" ]]; then
        log_info "Cleaning up previous test directory: $TEST_DIR"
        rm -rf "$TEST_DIR"
    fi
}

# Parse command line arguments
parse_arguments() {
    while [[ $# -gt 0 ]]; do
        case $1 in
            --languages)
                if [[ -n "$2" && "$2" != --* ]]; then
                    IFS=',' read -ra LANGUAGES <<< "$2"
                    shift 2
                else
                    log_error "Error: --languages requires a comma-separated list of languages"
                    exit 1
                fi
                ;;
            --query-kinds)
                if [[ -n "$2" && "$2" != --* ]]; then
                    IFS=',' read -ra QUERY_KINDS <<< "$2"
                    shift 2
                else
                    log_error "Error: --query-kinds requires a comma-separated list of query kinds"
                    exit 1
                fi
                ;;
            --run-tests)
                if [[ -n "$2" && "$2" != --* ]]; then
                    RUN_TESTS="$2"
                    shift 2
                else
                    log_error "Error: --run-tests requires a boolean value (true/false)"
                    exit 1
                fi
                ;;
            --help|-h)
                echo "Usage: $0 [--languages LANG1,LANG2,...] [--query-kinds KIND1,KIND2,...] [--run-tests true/false]"
                echo ""
                echo "Options:"
                echo "  --languages LANG1,LANG2,...    Comma-separated list of languages to validate"
                echo "                                 Default: python,java,javascript,go,cpp,csharp,ruby"
                echo "  --query-kinds KIND1,KIND2,...  Comma-separated list of query kinds to validate"
                echo "                                 Default: problem,path-problem"
                echo "  --run-tests true/false         Whether to run unit tests (default: true)"
                echo "  --help, -h                     Show this help message"
                echo ""
                echo "Environment variables:"
                echo "  TEST_DIR                       Directory for test files (default: /tmp/qlt-cli-e2e-test)"
                echo "  RUN_TESTS                      Whether to run unit tests (default: true)"
                echo ""
                echo "Supported languages: ${DEFAULT_LANGUAGES[*]}"
                echo "Supported query kinds: ${DEFAULT_QUERY_KINDS[*]}"
                exit 0
                ;;
            *)
                log_error "Unknown option: $1"
                echo "Use --help for usage information"
                exit 1
                ;;
        esac
    done

    # Validate that all specified languages are supported
    for lang in "${LANGUAGES[@]}"; do
        if [[ ! " ${DEFAULT_LANGUAGES[*]} " =~ " ${lang} " ]]; then
            log_error "Unsupported language: $lang"
            log_error "Supported languages: ${DEFAULT_LANGUAGES[*]}"
            exit 1
        fi
    done

    # Validate that all specified query kinds are supported
    for kind in "${QUERY_KINDS[@]}"; do
        if [[ ! " ${DEFAULT_QUERY_KINDS[*]} " =~ " ${kind} " ]]; then
            log_error "Unsupported query kind: $kind"
            log_error "Supported query kinds: ${DEFAULT_QUERY_KINDS[*]}"
            exit 1
        fi
    done

    # Validate run-tests parameter
    if [[ "$RUN_TESTS" != "true" && "$RUN_TESTS" != "false" ]]; then
        log_error "Invalid value for RUN_TESTS: $RUN_TESTS"
        log_error "Supported values: true, false"
        exit 1
    fi
}

# Setup trap for cleanup
# Note: We don't clean up at exit to allow troubleshooting of generated files

# Setup test environment
setup_test_environment() {
    log_info "Setting up test environment..."

    # Clean and create test directory
    cleanup_at_start
    mkdir -p "$TEST_DIR"
    cd "$TEST_DIR"

    # Initialize CodeQL workspace
    log_info "Initializing CodeQL workspace..."
    dotnet "$QLT_BINARY" query init

    # Create qlt.conf.json with latest CodeQL version
    log_info "Creating qlt.conf.json configuration..."
    cat > qlt.conf.json << 'EOF'
{
  "CodeQLCLI": "2.22.4",
  "CodeQLStandardLibrary": "codeql-cli/v2.22.4",
  "CodeQLCLIBundle": "codeql-bundle-v2.22.4",
  "EnableCustomCodeQLBundles": false,
  "CodeQLStandardLibraryIdent": "codeql-cli_v2.22.4"
}
EOF

    # Install CodeQL if not already installed
    log_info "Installing CodeQL..."
    dotnet "$QLT_BINARY" codeql run install || true

    log_success "Test environment setup complete"
}

# Generate queries for all languages and query kinds
generate_all_queries() {
    log_info "Generating queries for all languages and query kinds..."

    for language in "${LANGUAGES[@]}"; do
        for kind in "${QUERY_KINDS[@]}"; do
            local pack_name="testpack-$language"
            # Generate query name with kind suffix to match expected validation
            case "$kind" in
                "problem")
                    local query_name="TestQueryKindProblem"
                    ;;
                "path-problem")
                    local query_name="TestQueryKindPathProblem"
                    ;;
                *)
                    local query_name="TestQueryKind${kind}"
                    ;;
            esac

            log_info "  → Generating $kind query for $language..."
            if ! dotnet "$QLT_BINARY" query generate new-query \
                --language "$language" \
                --query-name "$query_name" \
                --pack "$pack_name" \
                --query-kind "$kind"; then
                log_error "Failed to generate $kind query for $language"
                return 1
            fi
        done
    done

    log_success "All queries generated successfully"
}

# Install all QL packs (runs once for entire workspace)
install_all_packs() {
    log_info "Installing QL packs for all languages..."
    if ! dotnet "$QLT_BINARY" query run install-packs; then
        log_error "Failed to install packs"
        return 1
    fi
    log_success "All packs installed successfully"
}

# Run unit tests using the dedicated script
run_unit_tests_for_languages() {
    local languages="$1"
    local work_dir="$2"

    log_info "Running unit tests for languages: $languages"

    # Get the directory where this script is located
    local script_dir="$SCRIPT_DIR"
    local unit_test_script="$script_dir/run-unit-tests.sh"

    if [[ ! -f "$unit_test_script" ]]; then
        log_error "Unit test script not found: $unit_test_script"
        return 1
    fi

    if [[ ! -x "$unit_test_script" ]]; then
        log_error "Unit test script is not executable: $unit_test_script"
        return 1
    fi

    # Run the unit tests script
    if "$unit_test_script" --test-dir "$work_dir" --languages "$languages"; then
        log_success "Unit tests passed : --languages $languages"
        return 0
    else
        log_error "Unit tests failed : --languages $languages"
        return 1
    fi
}

# Validate single language
validate_language() {
    local language="$1"
    local pack_name="testpack-$language"

    log_info "Validating language: $language"

    # Validate queries (includes formatting, compilation, etc.)
    log_info "  → Validating queries for $language..."

    # Capture validation output to distinguish warnings from errors
    local validation_output
    validation_output=$(dotnet "$QLT_BINARY" validation run check-queries --language "$language" 2>&1)
    local validation_exit_code=$?

    # Display the output for transparency
    echo "$validation_output"

    # Check if there are actual errors (not just deprecation warnings)
    if [[ $validation_exit_code -ne 0 ]]; then
        # Check if the failures are only deprecation warnings about assume_small_delta
        if echo "$validation_output" | grep -v "pragma 'assume_small_delta' is deprecated" | grep -q -E "(ERROR|FAILURE|Failed to|Error:|Compilation failed)"; then
            log_error "Query validation failed for $language with actual errors"
            return 1
        else
            log_info "Query validation completed for $language (only deprecation warnings in standard library)"
        fi
    else
        log_info "Query validation passed for $language"
    fi

    # Step 4: Verify generated files exist and have correct content for each query kind
    log_info "  → Verifying generated files for $language..."

    local lang_dir="$language/$pack_name"
    local src_dir="$lang_dir/src"
    local test_dir="$lang_dir/test"

    # Check query files for each kind
    for kind in "${QUERY_KINDS[@]}"; do
        local query_name="TestQuery"
        local clean_name

        # Generate expected clean name based on kind
        case "$kind" in
            "problem")
                clean_name="TestQueryKindProblem"
                ;;
            "path-problem")
                clean_name="TestQueryKindPathProblem"
                ;;
            *)
                clean_name="TestQueryKind${kind}"
                ;;
        esac

        # Check query file
        if [[ ! -f "$src_dir/$clean_name/$clean_name.ql" ]]; then
            log_error "Query file not found: $src_dir/$clean_name/$clean_name.ql"
            return 1
        fi

        # Check test files
        if [[ ! -f "$test_dir/$clean_name/$clean_name.qlref" ]]; then
            log_error "Test qlref file not found: $test_dir/$clean_name/$clean_name.qlref"
            return 1
        fi

        # Verify qlref content
        local expected_qlref="$clean_name/$clean_name.ql"
        local actual_qlref=$(cat "$test_dir/$clean_name/$clean_name.qlref")
        if [[ "$actual_qlref" != "$expected_qlref" ]]; then
            log_error "qlref content mismatch for $language $kind. Expected: '$expected_qlref', Got: '$actual_qlref'"
            return 1
        fi

        # Verify test source file exists and contains appropriate content for the language
        local test_extensions=("py" "java" "js" "go" "cpp" "cs" "rb")
        local test_found=false

        for ext in "${test_extensions[@]}"; do
            if [[ -f "$test_dir/$clean_name/$clean_name.$ext" ]]; then
                test_found=true
                local test_content=$(cat "$test_dir/$clean_name/$clean_name.$ext")

                # Verify it's not CodeQL syntax (common mistake in templates)
                if echo "$test_content" | grep -q "import.*ql\|from.*select"; then
                    log_error "Test file contains CodeQL syntax instead of $language source code"
                    return 1
                fi

                # Verify it contains some basic content
                if [[ -z "$test_content" ]] || [[ ${#test_content} -lt 10 ]]; then
                    log_error "Test file appears to be empty or too short"
                    return 1
                fi

                break
            fi
        done

        if [[ "$test_found" != true ]]; then
            log_warning "No test source file found for $language $kind query (this may be expected for some languages)"
        fi

        # Verify the query has the correct @kind metadata
        local query_content=$(cat "$src_dir/$clean_name/$clean_name.ql")
        if ! echo "$query_content" | grep -q "@kind $kind"; then
            log_error "Query $clean_name does not have correct @kind metadata. Expected: @kind $kind"
            return 1
        fi

        log_success "  ✓ $kind query validated for $language"
    done

    # Run unit tests if enabled
    if [[ "$RUN_TESTS" == "true" ]]; then
        log_info "  → Running unit tests for $language..."
        if run_unit_tests_for_languages "$language" "$TEST_DIR"; then
            log_success "  ✓ Unit tests passed for $language"
        else
            log_error "Unit tests failed for $language"
            return 1
        fi
    else
        log_info "  → Skipping unit tests for $language (disabled)"
    fi

    log_success "Language $language validation complete"
    return 0
}

# Main validation function
main() {
    # Parse command line arguments
    parse_arguments "$@"

    log_info "Starting end-to-end CLI validation..."
    log_info "Repository: $REPO_ROOT"
    log_info "Test directory: $TEST_DIR"
    log_info "Languages: ${LANGUAGES[*]}"
    log_info "Query kinds: ${QUERY_KINDS[*]}"
    log_info "Run tests: $RUN_TESTS"

    # Check if QLT binary exists
    if [[ ! -f "$QLT_BINARY" ]]; then
        log_error "QLT binary not found at: $QLT_BINARY"
        log_error "Please build the project first: dotnet build"
        exit 1
    fi

    # Setup test environment
    setup_test_environment

    # Generate all queries first
    if ! generate_all_queries; then
        log_error "Failed to generate queries"
        exit 1
    fi

    # Install all packs once
    if ! install_all_packs; then
        log_error "Failed to install packs"
        exit 1
    fi

    # Track results
    local passed=0
    local failed=0
    local failed_languages=()

    # Validate each language
    for language in "${LANGUAGES[@]}"; do
        if validate_language "$language"; then
            ((passed++))
        else
            ((failed++))
            failed_languages+=("$language")
        fi
    done

    echo
    log_info "=== Validation Summary ==="
    log_success "Number of Languages Passed: $passed"
    if [[ $failed -gt 0 ]]; then
        log_error "Number of Languages Failed: $failed"
        if [[ ${#failed_languages[@]} -gt 0 ]]; then
            log_error "Failed languages: ${failed_languages[*]}"
        fi
    else
        log_success "All languages passed validation and unit testing."
    fi

    if [[ $failed -eq 0 ]]; then
        log_success "All validations passed! 🎉"
        log_info "Generated files are available in: $TEST_DIR"
        exit 0
    else
        log_error "Some validations failed. 😞"
        log_info "Generated files are available for troubleshooting in: $TEST_DIR"
        exit 1
    fi
}

# Run main function
main "$@"
