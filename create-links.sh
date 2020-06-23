#!/usr/bin/env bash
set -eu

err_report() {
    echo "Error on line $1" >&2
}

trap 'err_report $LINENO' ERR

pushd "$(dirname "$0")"
SOLUTION_DIR="$(pwd)"
echo "SOLUTION_DIR: $SOLUTION_DIR"
ln -sfn "$SOLUTION_DIR/GolfClient/Assets/Scripts/Shared" "$SOLUTION_DIR/GolfServer/Assets/Scripts/Shared"
ln -sfn "$SOLUTION_DIR/GolfClient/Assets/Scenes/Levels" "$SOLUTION_DIR/GolfServer/Assets/Scenes/Levels"
echo "OK"
