# Berlin, still moving?

A playful status page tracking how many completed Berlin calendar days BVG and
DB-operated S-Bahn/regional services have run without transport disruptions.

The application consists of:

- a React/Vite frontend;
- two .NET 10 Lambda handlers (scheduled collection and read API);
- DynamoDB for current state and daily history;
- API Gateway, EventBridge Scheduler, S3, and CloudFront;
- Terraform and GitHub Actions for deployment.

Terraform owns Lambda infrastructure and configuration. GitHub Actions owns the
.NET application ZIP deployment for both functions.

API Gateway applies a best-effort throttle of 5 requests per second with bursts
up to 10 requests. Requests over the limit receive HTTP 429. This reduces
application load and cost but is not a replacement for AWS Shield or WAF when
stronger DDoS controls are required.

Records are measured from the deployment date. Public transport web pages are
provisional data sources and are surfaced as unavailable rather than clean when
collection fails.

## Local development

```bash
npm install
npm test
npm run dev --workspace frontend
```

Set `VITE_API_URL` to the deployed `/status` endpoint. See
[`docs/deployment.md`](docs/deployment.md) for AWS deployment prerequisites and
current source limitations.
