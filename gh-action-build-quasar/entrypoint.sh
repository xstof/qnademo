#!/bin/sh -e

echo "::debug current directory $(pwd)"

# Set work directory
if [ -n "${WORK_DIR}" ]; then
  echo "::debug changing working directory to: $WORK_DIR"
  cd $WORK_DIR
fi

exec $@