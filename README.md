# BandLab Posts API Implementation

The posts API implement the specification required for a test assignment. 

1. Create a post with an image and caption
2. Add a comment on a post
3. Delete a comment on a post
4. Get paginated (cursor based) posts ordered by number of comments in descending order with most recent two comments

There are two parts for the implementation:

- API 

  The API implements all API endpoints requested in test and implemented in ASP.NET and deployed in AWS ECS Fargate service.

- Event Listener

  The event listener processes DynamoDB stream events. This is used to convert and resize images. Also process comments to build a DynamoDB GSI for querying to support GET API endpoint.

## Tech Stack

- .Net 6.0
- AWS: ECS Fargate, lambda, S3, DynamoDB, DynamoDB streams, etc...
- Docker

## Continuous Integration and Delivery

CI/CD is done in [Github actions](https://github.com/needo-global/bandlab-blog-api/actions).
I directly used the 'develop' branch for changes, but generally we should use a feature branch for development and then merge to the develop branch via a pull request.
Also we can follow the trunk based development with a main branch and a set of feature branches.

## High level Architecture

### NOTE: We can upgrade the solution to use an ElasticSearch cluster for querying purposes. So all DynamoDB stream events could be pushed to the cluster.

![Alt text](https://bandlab-post-dev-data.s3.ap-southeast-2.amazonaws.com/bandlab-api-architecture.PNG)

## How to run the project

In order to run the project locally we need set up some extra infrastructure as we are using AWS resources. 
For this LocalStack (https://www.localstack.cloud/) can be used. But this takes more time and added as a TODO.

### TODO - Integrate LocalStack to mock AWS resources

## Test DEVELOPMENT API

The test API is hosted on a subdomain. The subdomain is https://api.needo.com.au (This is my personal domain where I hosted a website. Check it out @ https://needo.com.au.
For example to create a post, make a POST request to https://api.needo.com.au/posts with a payload. No authorization is required at the moment (TODO).
The POST payload is form data object with 'Image' file and 'Caption' string.

Use the postman collection and environment in 'Postman' folder under the root folder.

## Test cases

There is no extensive test coverage as writing tests consume a significant time. But during normal development we should write unit tests, integration tests and end-to-end tests. I have done a model test that I would follow for unit tests using mocks.

## Production readiness steps

I have skipped number of concerns while implementing this API. But requires MANDATORY follow up for production readiness.

Formost important thing is test coverage to a satisfiable level.

1. Add more tests and increase coverage
2. Add authorization - Either using a API key or OAUTH2 protocol
3. Add caching layer to support query results
4. Add observability through logging and monitoring
5. Add documentation
6. Add Swagger support
7. Post images are public - secure them if public is not safe
8. Scanning API payloads for security vulnerabilities (like images, etc...)
9. Use configuration options for ECS and Lambda to remove hardcoded constants
10. Fix 'TODO' comments
11. Consider using ElasticSearch (or AWS Opensearch) domain for querying purpose
