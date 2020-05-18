#!/usr/bin/env bash
set -eu

err_report() {
    echo "Error on line $1" >&2
}

trap 'err_report $LINENO' ERR

pushd "$(dirname "$0")"
SOLUTION_DIR="$(pwd)"
echo "SOLUTION_DIR: $SOLUTION_DIR"
ln -sfn "$SOLUTION_DIR/GolfClient/Assets" "$SOLUTION_DIR/GolfServer/Assets/ClientAssets"
echo "OK"
