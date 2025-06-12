#!/bin/bash
mongod --auth --replSet rs0 --keyFile /etc/mongo/mongo-keyfile --bind_ip_all

mongo --eval "rs.initiate()"