#!/bin/bash
echo "Creating SQS queue: product-created"
awslocal sqs create-queue --queue-name product-created