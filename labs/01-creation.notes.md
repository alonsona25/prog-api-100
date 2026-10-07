# Some Notes

## Creating a local development environment

- Reproducibility
    - but also in your pipeline
- Dev/Prod Parity

Other ways:
    - Install a bunch of stuff (databases, etc. on your developer machine)
    - Have a "shared" development environment (databases that the whole team shares and works from for development)
    - Docker Compose files (did this for a *long* time)
    - Local Kubernetes Clusters...


## Service Defaults

The configuration for your API that is defined outside the work of your particular API. Cross-cutting things across all services you create.

- Proxy configuration
- Site Reliability Engineering (SRE)
    - Retries, circuit breakers and other "policies" ("polly")


## OpenApi Documentation for our API

Manual testing should (IMO) always happen *via* the OpenApi docs.

Alternatives (big one) SwaggerUI


## File organization

- What are "models"
- Why records for models?
- What is an "Entity"?

