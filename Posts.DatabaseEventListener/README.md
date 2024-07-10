# Bulk Virtual Account Closure

Provides a job to close virtual account in bulk. Currently, this is a manually triggered job that is built specically for closing dormant account. It is very simple and does not provide any validation if virtual account should be closed or not. It is the user responsibility to check for dormant account criteria prior to invoking this job.

## Getting Started

Once you have identified the virtual accounts that you want to close, note down their ids. You will need them to invoke the job.

To invoke the job, go to GCA AWS Commands Repository > Actions > [Lambda Invoke](https://github.com/ofx-com/gca-aws-commands/actions/workflows/lambda-invoke.yml).

TBC...

## How-to develop

### Prerequisite

- Framework: .Net 6
- [Mock Lambda Test Tool 6.0] (https://github.com/aws/aws-lambda-dotnet/tree/master/Tools/LambdaTestTool)
- Visual Studio 2022
- Optional: Visual Studio Code

### Run locally

In directory `GCA.BulkVirtualAccountClosure`, run:

- Setup user secrets
  ```pwsh
  dotnet user-secrets "Okta:ClientSecret" "<get_it_from_your_friendly_peers>"
  dotnet user-secrets "Okta:Password" "<get_it_from_your_friendly_peers>"
  ```
- Run mock lambda test tool
  ```pwsh
  dotnet-lambda-test-tool-6.0.exe
  ```
- Set `GCA.BulkVirtualAccountClosure` as startup project
- Hit `F5` to run and debug the project. Alternatively, you can use `dotnet watch`

Once it is running, go to Mock Lambda Test Tool > Executable Assembly and start queueing the event. This should hit your project.
