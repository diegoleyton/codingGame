#!/bin/bash
set -e

UNITY="/Applications/Unity/Hub/Editor/6000.0.26f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
TEMP_DIR="$PROJECT_PATH/Temp"

mkdir -p "$TEMP_DIR"

"$UNITY" \
  -batchmode \
  -quit \
  -projectPath "$PROJECT_PATH" \
  -runTests \
  -testPlatform EditMode \
  -testResults "$TEMP_DIR/unity-test-results.xml" \
  -logFile "$TEMP_DIR/unity-test.log"

echo "Unity EditMode tests passed."