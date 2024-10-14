# C# Cloud AWS - Day Three

## Learning Objectives
   - Understand how to deploy a backend using AWS Lambda.
   - Learn how to expose Lambda functions using API Gateway.
   - Automate AWS Lambda deployments using Terraform.
   - (Optional) Split backend functionalities into separate Lambda functions, triggered via HTTP calls.

## Instructions

1. Fork this repository.
2. Clone your fork to your machine.

# Core Activity

## Deploy Backend Using AWS Lambda and API Gateway
### Steps
1. Create a Lambda Function:
   - Open the AWS Management Console and navigate to Lambda.
   - Click Create function.
   - Choose Author from scratch.
   - Name the function (e.g., MyBackendFunction).
   - Select the C# (.NET Core) runtime.
   - Click Create function.

2. Create an AWS Cli Access Key for your account
   - navigate to AWS IAM
   - under `Users`, find your username and click on it
   - visit the `Security Credentials` tab
   - Click on `Create Access Key`
   - Choose `Command Line Interface` as the Use Case
   - Save the Access Key safely on your machine
   - Click `show` and copy and save the Secret access key as well

3. Open the AWSLambda-Day-3 Solution
   - Install the Extension called AWS Toolkit in Visual Studio.
   - Once installed, open up AWS Explorer in VS: Views > AWS Explorer
   - Add new Profile with your User Credentials
      - Profile Type is `IAM User Role`
      - Profile Name: choose your name or some identifier tied to you
      - Enter the Access key and Secret key obtained in step 2 above
      - Ensure you pick the default region for your AWS services

4. Publish Application to AWS Lambad
   - Right click on Project and choose `Publish Lambda function to AWS`
   - Make sure to select the created profile in step 3 and the correct region
   - Choose `Redeploy to existing` and find the lambda you created in step 1
   - Click `Upload`
   OR
   - You can publish to folder and zip and upload manually

4. Configure function URL
   - Navigate into the function and go to confguration TAB
   - Click on Function URL and Enable and Choose `NONE` option for Auth Type
   - Save


## Use Terraform to Automate Lambda Deployment
## Prerequisites:
   -[] Ensure that Terraform CLI is installed and AWS CLI is configured.
   - [Terraform](https://developer.hashicorp.com/terraform/install)
     - Add the Terraform.exe to your `C:\Program Files\Git\usr\bin` (If you have git Bash install.)
     - Otherwise and directory of your choice.
     - Add the Directory to your Environment Variable `%PATH%`
     - Steps on how to do it here [Setup of Terraform in Environment Variables](https://stackoverflow.com/a/55949463)
   - [AWS Terraform](https://developer.hashicorp.com/terraform/tutorials/aws-get-started)

### Steps
1. Write Terraform Configuration:
    - In your project folder, create a file named main.tf with the following content:
    ```bash
      provider "aws" {
         region = "eu-north-1"
      }

      resource "aws_iam_role" "lambda_role" {
         name = "lambda_role"
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
         function_name    = "MyBackendFunction"
         role             = aws_iam_role.lambda_role.arn
         handler          = "Backend::Backend.Function::FunctionHandler"
         runtime          = "dotnet6"
         source_code_hash = filebase64sha256("backend.zip")
      }

      resource "aws_api_gateway_rest_api" "api" {
         name = "MyBackendAPI"
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
   ```

2. Package Your Lambda Code:
   - Publish your Lambda function using the following commands:
   ```bash
   dotnet publish -c Release -o out
   cd out
   zip -r backend.zip .

   ```

3. Run Terraform:
   - Initialize Terraform:
   ```bash
   terraform init
   ```
   - Apply the configuration:
   ```bash
   terraform apply
   ```
4. Verify Lambda and API Gateway Deployment:
   - Check the API Gateway endpoint URL in AWS and ensure it is connected to the Lambda function by visiting `https://your-api-id.execute-api.region.amazonaws.com/register`.

## (Optional) Split Functionalities into Multiple Lambda Functions
### Steps

1. Create Additional Lambda Functions:
   - Create separate Lambda functions to handle different tasks. For example, create one function for user registration and another for order processing.

2. Update API Gateway:
   - In API Gateway, add new routes like `/register` and `/order`, linking each route to its respective Lambda function.

3. Test the API:
   - Verify that different routes trigger different Lambda functions. For instance:
   - `/register` calls the user registration function.
   - `/order` calls the order processing function.
