#!/bin/bash
chown mongodb:mongodb /etc/mongo/mongo-keyfile
chmod 400 /etc/mongo/mongo-keyfile

exec "$@"