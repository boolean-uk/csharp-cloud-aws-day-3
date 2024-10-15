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
1. Create an AWS Cli Access Key for your account
   - navigate to AWS IAM
   - under `Users`, find your username and click on it
   - visit the `Security Credentials` tab
   - Click on `Create Access Key`
   - Choose `Command Line Interface` as the Use Case
   - Save the Access Key safely on your machine
   - Click `show` and copy and save the Secret access key as well

2. Give yourself Admin
   - navigate to _elevate folder
   - run MakeAdmin.cmd
   - restart your PC to apply Admin privilages

3. Open the AWSLambda-Day-3 Solution
   - Install the Extension called AWS Toolkit in Visual Studio.
   - Close VS to install the extension.
   - Install the Extension called AWS Toolkit in Visual Studio Code as well.
   - Press AWS in Visual Studio Code, Select `IAM User Role`
   - Profile Name: Personal email
   - Access Key: Generated from step 1
   - Secret Access Key: Generated from step 1
   - After you are logged in, close VS: Code
   - Open up AWS Explorer in VS. View > AWS Explorer
   - Edit Profile (You should be automatically logged in from VS: Code)
      - Edit Profile Region from US Virginia to Europe Stockholm

4. Required for Publishing to work
   - Open the sln file in VS. Double-click the project file `AWSLambda-Day-3`
   - Under `<PublishReadyToRun>true</PublishReadyToRun>` Add this line: `<RuntimeIdentifier>win-x64</RuntimeIdentifier>`

## Use Terraform to Automate Lambda Deployment
## Prerequisites:
   -Download Terraform
   - [Terraform](https://developer.hashicorp.com/terraform/install)
     - Move the Terraform.exe to your `C:\Program Files\Git\usr\bin`
     - Open `Edit the system environment variables` (Windows Search)
     - In the System Properties window. Press `Environment Variables...`
     - At the top of the new window, select `Path` and press `Edit...`
     - In Edit environment variable, select `New` and type: `C:\Program Files\Git\usr\bin`
     - Hit `OK`, `OK`, and `OK` to close all the windows.

### Steps
1. Write Terraform Configuration:
    - In your project folder, create a file named main.tf with the following content:
    - Replace `{Personal Email}` with your personal email found in `https://us-east-1.console.aws.amazon.com/iam/home?region=eu-north-1#/users`
    - Replace `{Lambda Name}` with a name of your choice. (This will be the name of your Lambda Function)
    - Replace `{API Name}` with a name of your choice. (This will be the name of your API Gateway)
    ```bash
   provider "aws" {
      region = "eu-north-1"
   }

   resource "aws_iam_role" "lambda_role" {
      name = "{Personal Email}"
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
      function_name    = "{Lambda Name}"
      role             = aws_iam_role.lambda_role.arn
      handler          = "AWSLambda-Day-3::AWSLambda_Day_3.Function::FunctionHandler"
      runtime          = "dotnet8"
      source_code_hash = filebase64sha256("backend.zip")
   }

   resource "aws_api_gateway_rest_api" "api" {
      name = "{API Name}"
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
   - Publish your Lambda function using the following commands (Git Bash):
   ```bash
   dotnet publish -c Release -o out
   ```
   - Open the generated `out` folder in File Explorer and ZIP together all files into `backend.zip`

3. Set up the folder correctly:
   - Move both `backend.zip` and `main.tf` into the folder that contains the folder `AWSLambda-Day-3` and `AWSLambda-Day-3.sln`. 

4. Set up AWS CLI:
   - Run the following code in Git Bash:
   ```bash 
   curl "https://awscli.amazonaws.com/AWSCLIV2.msi" -o "AWSCLIV2.msi"
   ```
   - Open the `AWSCLIV2.msi` file and run the installer.
   - Once the installation is finished. Delete the `AWSCLIV2.msi` file
   - Open up `cmd` and run:
   ```bash
   aws configure
   ```
   - Configure as such:
     - AWS Acess Key ID = `{Your_Access_Key_From_Step_1}`
     - AWS Secret Access Key = `{Your_Secret_Access_Key_From_Step_1}`
     - Default region name = `eu-north-1`
     - Default output format. Just hit enter here, leave this empty.
   - AWS CLI should now be set up. Close your `cmd`

5. Run Terraform:
   - Navigate Git Bash so you are in the same folder that contains `AWSLambda-Day-3`, `AWSLambda-Day-3.sln`, `main.tf`, and `backend.zip`.
   - Initialize Terraform:
   ```bash
   terraform init
   ```
   - Apply the configuration:
   ```bash
   terraform apply
   ```

6. Publish Application to AWS Lambda
   - Go to VS and open up your sln file.
   - Right click on Project `AWSLambda-Day-3` and choose `Publish to AWS Lambda...`
   - Choose `Re-deploy to existing` and find the lambda you created `{Lambda Name}`
   - Click `Upload`

Congratulations, Terraform is now done!

## AWS Finalization Steps

1. Configure API Gateway
   - Find your `{API Name}` in `https://eu-north-1.console.aws.amazon.com/apigateway/main/apis?api=unselected&region=eu-north-1` and open it.
   - Select `Deploy API`
     - `*New Stage*`
     - Enter a Stage Name. Keep it short, e.g `prod`.
     - Press `Deploy`.

2. Configure Lambda Function
   - Find your `{Lambda Name}` in `https://eu-north-1.console.aws.amazon.com/lambda/home?region=eu-north-1#/functions` and open it.
   - Select `Configuration`
     - Select `Function URL`
       - Select `Create Function URL`
         - Select `NONE`
         - Press `Save`
     - Select `Triggers`
       - Select `Add trigger`
         - Select `API Gateway` as source
         - Select `Use existing API`
         - Select `{API Name}` as API ID
         - Select your deployment stage
         - Set Security to `Open`
         - Press `Add`

3. Verify Lambda and API Gateway Deployment:
   - In the `Triggers` Section under `Configuration` on your Lambda function you should now find a linked API Gateway. Ensure that this works by going to the `API Endpoint` link.
   - If you see `1. "Hello world!"` you did everything correctly! HURRAY!

P.S Make sure to take screenshots of the results and create a pull request with your screenshots to show you did it!

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
