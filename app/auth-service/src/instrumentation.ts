import { NodeSDK } from "@opentelemetry/sdk-node";
import { SemanticResourceAttributes } from "@opentelemetry/semantic-conventions";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-proto";
import { HttpInstrumentation } from "@opentelemetry/instrumentation-http";
import { ExpressInstrumentation } from "@opentelemetry/instrumentation-express";
import { Resource } from "@opentelemetry/resources";

const jaegerHost = process.env.JAEGER_URI || "http://localhost:4318";

const resource =
    Resource.default().merge(
        new Resource({
            [SemanticResourceAttributes.SERVICE_NAME]: "AuthService",
        })
    );

const sdk = new NodeSDK({
    resource: resource,
    traceExporter: new OTLPTraceExporter({
        url: `${jaegerHost}/v1/traces`,
    }),
    instrumentations: [
        new HttpInstrumentation(),
        new ExpressInstrumentation(),
    ]
});

sdk.start()
