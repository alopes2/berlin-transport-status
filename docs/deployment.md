# Deployment

## Prerequisites

- AWS account and an OIDC deployment role trusted by GitHub Actions.
- Terraform state backend configuration appropriate for the AWS account.
- GitHub production environment secret `AWS_DEPLOY_ROLE_ARN`.
- The GitHub OIDC role must allow `lambda:UpdateFunctionCode` and
  `lambda:GetFunctionConfiguration` for both application functions, in addition
  to the Terraform and frontend deployment permissions.

## Initial deployment

1. Configure the Terraform backend, or run with local state for development.
2. Run the `Deploy` workflow and provide the first Berlin tracking date.
3. Terraform creates or updates infrastructure, including Lambda configuration,
   IAM, API Gateway, and EventBridge wiring. It uses a bootstrap ZIP only when a
   Lambda function is created and ignores application code afterward.
4. GitHub Actions publishes the .NET 10 ZIP and deploys it to both functions
   with `aws lambda update-function-code`.
5. EventBridge invokes the collector every five minutes.
6. The first successful collection creates the DynamoDB company records.

Records are measured from `tracking_since`; historical records are not inferred.

Terraform owns Lambda configuration but not Lambda application code. Running
`terraform apply` after a code deployment does not roll back the deployed ZIP.

## Source limitations

- BVG uses the public JSON request behind its disruption page and excludes
  accessibility-only messages.
- S-Bahn Berlin uses the public GraphQL request behind its disruption page and
  counts consequences whose time-frame includes the current Berlin day.
- DB Regio uses the public JSON request behind its regional disruption page,
  restricted to the current Berlin calendar day.
- Replace these provisional public interfaces with the VBB `HimSearch` API if
  production credentials become available.

No queue is required for v1. EventBridge Scheduler is sufficient because every
collection is idempotent and DynamoDB stores only the latest company aggregate.
