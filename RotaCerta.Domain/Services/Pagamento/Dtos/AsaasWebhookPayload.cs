using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RotaCerta.Domain.Services.Pagamento.Dtos
{
    public record AsaasWebhookPayload(
        [property: JsonPropertyName("event")] string Event,
        [property: JsonPropertyName("payment")] AsaasWebhookPayment? Payment);

    public record AsaasWebhookPayment(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("subscription")] string? Subscription,
        [property: JsonPropertyName("status")] string? Status);
}
