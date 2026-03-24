## Architectural Overview

This project showcases **dynamic secret management patterns** in a .NET 9 ASP.NET Core Web API deployed on Kubernetes. It progresses from basic environment variables to production-grade Vault Agent sidecars, proving how applications handle secret rotation at runtime.

## Core Objectives

- ✅ **Validate 3 secret delivery models** with runtime reload behavior
- ✅ **Demonstrate .NET configuration lifetimes** (IOptions → IOptionsMonitor)
- ✅ **Bootstrap production Vault infrastructure** on Kubernetes
- ✅ **Prove zero-downtime secret rotation** capabilities

## Secret Management Patterns

| Pattern | Manifest | Refresh Mechanism | Use Case |
|---------|----------|------------------|----------|
| **Env Vars** | `deployment-minimal.yaml` | Pod restart | Learning baseline |
| **Volume Mount** | `deployment-volumeMount.yaml` | File polling | File-based refresh |
| **Vault Agent** | `deployment-vault-agent.yaml` | Sidecar streaming | Production rotation |

## Key Features Demonstrated

### 🔄 Runtime Configuration Reload

```csharp
// PaymentController showcases all patterns
[HttpGet("payment")]
public IActionResult GetPaymentConfig()
{
    var config1 = _options.Value;           // IOptions (stale)
    var config2 = _optionsSnapshot.Value;   // IOptionsSnapshot (pod scope)
    var config3 = _optionsMonitor.CurrentValue; // IOptionsMonitor (live)
    // Returns active Stripe retry config
}
```
appsettings.json
└── /app/secrets/appsettings.json (mounted)
    └── Stripe:SecretKey (rotatable)
    
###☸️ Vault Kubernetes Bootstrap
Infra: vault-infrastructure.yaml (server deployment)

RBAC: vault-rbac.yaml (service account + auth path)

Init: vault-init-job.yaml (unseal + KVv2 setup)

| Endpoint             | Purpose                                      |
| -------------------- | -------------------------------------------- |
| GET /api/payment     | Returns live Stripe config (proves rotation) |
| GET /weatherforecast | .NET template endpoint                       |

# 1. Start Minikube/k3s
minikube start

# 2. Deploy Vault infrastructure
kubectl apply -f vault-*.yml

# 3. Test progression
kubectl apply -f deployment-minimal.yaml         # Static env
kubectl apply -f deployment-volumeMount.yaml     # File refresh
kubectl apply -f deployment-vault-agent.yaml     # Dynamic sidecar

### Deployment Progression
Static Env Vars → Volume Mount Polling → Vault Agent Sidecar
     ↓                ↓                     ↓
  Simple           File-based          Production-grade
 Learning         refresh             zero-downtime

Try it: Rotate secrets and watch /api/payment reflect changes instantly without restarts!
