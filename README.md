# BandLog Posts API Implementation

The posts API implement the specification required for test assignment. 

1. Create a post with an image and caption
2. Add comemnt on a post
3. Delete comment on a post
4. Get paginated (cursor based) posts ordered by number of comments in descending order with latest two comments

This microservice provides the integration services to share in-scope customer data with Amazon.

There are 2 parts of services:

- API 

  The API implement all API endpoints requested in test and implemented in ASP.NET and deployed in AWS ECS Fargate service.

- Event Listener

  The event listener is processing DynamoDB stream events. This is used to convert and resize images. Also process comments to build a DynamoDB GSI for later querying for GET API endpoint.

## Tech Stack

- .Net 6.0
- AWS: ECS Fargate, lambda, S3, DynamoDB
- Docker

## Continuous Delivery

CI/CD is done in [Github actions](https://github.com/needo-global/bandlab-blog-api/actions).

## High level Architecture

![Alt text](https://bandlab-post-dev-data.s3.ap-southeast-2.amazonaws.com/bandlab-api-architecture.PNG)

## How to run your project

// TODO - 

## Test DEVELOPMENT API

The test API is hosted in this domain. The domain is https://api.needo.com.au
For example to create a post, make POST request https://api.needo.com.au/posts with a payload.
The POST payload is form data object with 'Image' file and 'Caption' string.

Use the postman collection in 'Postman' folder in root folder.

## Test cases

There is no extensive test coverage as writing tests consume a significant time. But during normal development we should write unit tests, integration tests and end-to-end tests. I have done a model test that would follow for unit test using mocks.

## Production readiness steps

I have skipped number of concerns while implementing this API. But requires following for production readiness.

1. Add authorization - Either using a API key or OAUTH2 protocol
2. Add caching layer to support query results
3. Add observability through logging and monitoring
4. Add documentation
5. Add Swagger support
6. Post images are public - secure them if public in not safe
7. Scanning API payloads for security vulnerabilities (like images, etc...)
