#!/bin/bash
echo "Initializing LocalStack resources..."

# Create SQS queue
awslocal sqs create-queue \
  --queue-name product-created

echo "SQS queue 'product-created' created."

# Create DynamoDB table
awslocal dynamodb create-table \
  --table-name ProductStock \
  --attribute-definitions \
      AttributeName=productId,AttributeType=S \
      AttributeName=updatedAt,AttributeType=S \
  --key-schema \
      AttributeName=productId,KeyType=HASH \
      AttributeName=updatedAt,KeyType=RANGE \
  --billing-mode PAY_PER_REQUEST

echo "DynamoDB table 'ProductStock' created."

echo "LocalStack initialization complete."
