#!/bin/bash

docker run -d --name app -p 8080:8080 -v /home/core-bot/logs:/  vsepike/core-bot:latest