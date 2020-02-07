#!/bin/bash -e

# Set work directory
if [ -n "${WORK_DIR}" ]; then
  cd $WORK_DIR
fi

exec $@