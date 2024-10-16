provider "aws" {
  region = "eu-north-1"
}

resource "aws_iam_role" "lambda_role" {
  name = "ali_786@live.no"
  assume_role_policy = jsonencode({
     Version = "2012-10-17",
     Statement = [{
        Action = "sts:AssumeRole",
        Effect = "Allow",
        Principal = {
        Service = "lambda.amazonaws.com"
        }
     }]
  })
}

resource "aws_lambda_function" "backend" {
  filename         = "backend.zip"
  function_name    = "khan-lambdafunction"
  role             = aws_iam_role.lambda_role.arn
  handler          = "AWSLambda-Day-3::AWSLambda_Day_3.Function::FunctionHandler"
  runtime          = "dotnet8"
  source_code_hash = filebase64sha256("backend.zip")
}

resource "aws_api_gateway_rest_api" "api" {
  name = "khan_api"
}

resource "aws_api_gateway_resource" "resource" {
  rest_api_id = aws_api_gateway_rest_api.api.id
  parent_id   = aws_api_gateway_rest_api.api.root_resource_id
  path_part   = "register"
}

resource "aws_api_gateway_method" "method" {
  rest_api_id   = aws_api_gateway_rest_api.api.id
  resource_id   = aws_api_gateway_resource.resource.id
  http_method   = "GET"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "integration" {
  rest_api_id = aws_api_gateway_rest_api.api.id
  resource_id = aws_api_gateway_resource.resource.id
  http_method = aws_api_gateway_method.method.http_method
  type        = "AWS_PROXY"
  integration_http_method = "POST"
  uri         = aws_lambda_function.backend.invoke_arn
}