# ChallengeService API - Scalability and Architectural Design

This document addresses theoretical questions about scalability, load distribution, and high-volume system design, based on the architecture implemented in ChallengeService.

## 📋 Table of Contents

1. [Scalable API Design](#1-scalable-api-design)
2. [Mass Content Distribution](#2-mass-content-distribution)
3. [Large-Scale Data Queries](#3-large-scale-data-queries)

---

## 1. Scalable API Design

### Question
How would you design an API capable of responding to thousands of devices querying the same API every few minutes, maintaining low latency and high availability?

### Answer

#### Base Architecture: CQRS as Foundation

The architecture implemented in ChallengeService uses **CQRS (Command Query Responsibility Segregation)**, which is fundamental for scalability:

```
┌─────────────────────────────────────────────────────────┐
│                    Load Balancer                         │
│              (Load Distribution)                          │
└──────────────┬──────────────────────────────┬─────────────┘
               │                              │
    ┌──────────▼──────────┐      ┌──────────▼──────────┐
    │   API Instances      │      │   API Instances      │
    │   (Read/Write)      │      │   (Read Only)        │
    │   - Commands         │      │   - Queries          │
    └──────────┬──────────┘      └──────────┬──────────┘
               │                              │
    ┌──────────▼──────────┐      ┌──────────▼──────────┐
    │   Write Database    │      │   Read Replicas      │
    │   (SQL Server)       │      │   (SQL Server)       │
    └──────────────────────┘      └──────────────────────┘
```

#### Implemented and Recommended Strategies

##### 1. Read/Write Separation (CQRS)

**Current Implementation:**
- ✅ `Challenge.Commands` layer for writes (Create, Update, Delete operations)
- ✅ `Challenge.Queries` layer for reads (GetById, GetAll with pagination, filtering, and sorting)
- ✅ Complete decoupling through MediatR
- ✅ Queries implemented for Categories, Products, and ProductCategories

**Future Scalability:**
- **Separate Read Store**: Read-only database optimized for queries
- **Optimized Write Store**: Database optimized for transactions
- **Synchronization**: Event Sourcing or CDC (Change Data Capture) to maintain eventual consistency

**Advantages:**
- ✅ Independent scaling of reads and writes
- ✅ Specific optimization per operation type
- ✅ Reduced database contention

**Disadvantages:**
- ⚠️ Additional complexity in synchronization
- ⚠️ Eventual consistency (acceptable for most cases)

##### 2. Multi-Level Caching

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │
┌──────▼──────────────────────────────────────┐
│   CDN / Edge Cache (CloudFlare, AWS CloudFront) │
│   - Full response caching                     │
│   - TTL: 1-5 minutes                          │
└──────┬──────────────────────────────────────┘
       │
┌──────▼──────────────────────────────────────┐
│   Application Cache (Redis Cluster)          │
│   - Frequently accessed data cache          │
│   - TTL: 5-15 minutes                        │
│   - Event-driven invalidation                │
└──────┬──────────────────────────────────────┘
       │
┌──────▼──────────────────────────────────────┐
│   Database Query Cache                      │
│   - SQL query-level caching                 │
│   - Execution plans                        │
└─────────────────────────────────────────────┘
```

**Caching Strategy:**
- **Level 1 (CDN)**: Complete HTTP responses for static/semi-static data
- **Level 2 (Redis)**: Domain data frequently queried
- **Level 3 (DB)**: Complex SQL query caching

**Invalidation:**
- Event-driven: When writing, publish invalidation event
- Conservative TTL: 1-5 minutes for dynamic data
- Cache-aside pattern: Application manages cache

##### 3. Load Balancing and Auto-Scaling

```
┌─────────────────────────────────────────────┐
│         Application Load Balancer             │
│         (Health Checks, SSL Termination)     │
└──────────────┬───────────────────────────────┘
               │
    ┌───────────┴───────────┐
    │                       │
┌───▼────┐            ┌────▼───┐
│ API-1  │            │ API-2  │
│ (2 vCPU│            │ (2 vCPU│
│ 4GB)   │            │ 4GB)   │
└───┬────┘            └────┬───┘
    │                     │
    └───────────┬───────────┘
                │
         ┌──────▼──────┐
         │  Database   │
         │  (Primary)  │
         └─────────────┘
```

**Recommended Configuration:**
- **Load Balancer**: Round-robin with health checks
- **Auto-scaling**: Based on CPU (70%) and latency (<200ms)
- **Instances**: Minimum 2, maximum 10-20 depending on demand
- **Health Checks**: `/health` endpoint with DB verification

##### 4. Database Optimization

**Read Replicas:**
```
Primary DB (Write) ──┐
                     ├──> Replica 1 (Read)
                     ├──> Replica 2 (Read)
                     └──> Replica 3 (Read)
```

- **Connection Pooling**: Maximum 100 connections per instance
- **Query Optimization**: Indexes on frequently queried fields
- **Read Replicas**: 2-3 replicas to distribute read load
- **Partitioning**: Horizontal partitioning by ID range or dates

##### 5. Rate Limiting and Throttling

```csharp
// Recommended implementation
services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

**Strategies:**
- **Per IP**: 100 requests/minute per IP
- **Per User**: 200 requests/minute per authenticated user
- **Per Endpoint**: Specific limits based on criticality
- **Exponential Backoff**: For clients exceeding limits

##### 6. Monitoring and Observability

- **APM (Application Performance Monitoring)**: Application Insights, New Relic
- **Key Metrics**:
  - P95, P99 Latency
  - Throughput (requests/second)
  - Error rate
  - Database connection pool usage
  - Cache hit ratio
- **Alerts**: Latency > 500ms, Error rate > 1%, CPU > 80%

#### Solution Summary

| Strategy | Current Implementation | Future Scalability |
|----------|----------------------|-------------------|
| CQRS | ✅ Commands/Queries separation | Separate Read/Write stores |
| Caching | ⚠️ Not implemented | Redis Cluster + CDN |
| Load Balancing | ⚠️ Manual | Auto-scaling with ALB |
| Read Replicas | ⚠️ Not implemented | 2-3 SQL Server replicas |
| Rate Limiting | ⚠️ Not implemented | Per IP/User/Endpoint |
| Monitoring | ⚠️ Basic (Serilog) | Complete APM |

---

## 2. Mass Content Distribution

### Question
What strategies would you implement to ensure that thousands of devices download new content without creating network or server bottlenecks?

### Answer

#### Content Distribution Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Content Origin                       │
│              (ChallengeService API + Storage)                │
└──────────────┬──────────────────────────────────────────┘
               │
               │ Upload/Update
               │
    ┌──────────▼──────────────────────────┐
    │      Content Distribution Network      │
    │      (CloudFlare / AWS CloudFront)     │
    │                                        │
    │  ┌────────┐  ┌────────┐  ┌────────┐  │
    │  │ Edge 1 │  │ Edge 2 │  │ Edge N │  │
    │  │ (US-E) │  │ (EU-W) │  │ (AP-S) │  │
    │  └────────┘  └────────┘  └────────┘  │
    └──────────┬───────────────────────────┘
               │
    ┌──────────▼──────────────────────────┐
    │         Thousands of Devices         │
    │    (Download from nearest Edge)      │
    └──────────────────────────────────────┘
```

#### Implemented and Recommended Strategies

##### 1. CDN (Content Delivery Network)

**Current Implementation:**
- ✅ Static file service (`UseStaticFiles`)
- ✅ Local storage in `wwwroot/images/products/`
- ⚠️ **Limitation**: Served from origin server

**Production Recommendation:**

```csharp
// Recommended configuration
services.Configure<StaticFileOptions>(options =>
{
    options.OnPrepareResponse = ctx =>
    {
        // Headers for CDN caching
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
        ctx.Context.Response.Headers.Append("CDN-Cache-Control", "public,max-age=31536000");
    };
});
```

**CDN Strategy:**
- **CloudFlare / AWS CloudFront**: Global distribution
- **Edge Locations**: 200+ worldwide locations
- **Caching**: 1-year TTL for images (manual invalidation)
- **Compression**: Automatic Gzip/Brotli
- **HTTPS**: SSL/TLS on all edge locations

**Advantages:**
- ✅ Latency reduction (serve from nearest location)
- ✅ Origin server load reduction (90-95%)
- ✅ Higher global throughput
- ✅ DDoS protection included

**Disadvantages:**
- ⚠️ Additional cost (but minimal compared to bandwidth)
- ⚠️ Cache invalidation requires specific API

##### 2. Object Storage (Blob Storage)

**Recommended Migration:**

```
Current: wwwroot/images/products/ (Local)
         ↓
Future: Azure Blob Storage / AWS S3
         ↓
         CDN (CloudFlare / CloudFront)
         ↓
         Devices
```

**Azure Blob Storage:**
- **Containers**: Organized by type (products, categories)
- **Naming**: `{guid}.{ext}` (already implemented)
- **Tiers**: Hot (frequent), Cool (archive), Archive (backup)
- **Lifecycle Policies**: Move to Cool after 30 days without access

**Advantages:**
- ✅ Unlimited scalability
- ✅ Automatic redundancy (3 copies by default)
- ✅ Native CDN integration
- ✅ Pay-per-use (very economical)

##### 3. Invalidation and Update Strategy

**Problem**: Thousands of devices querying new content

**Solution: Smart Polling + Webhooks**

```
┌─────────────┐
│   Device    │
└──────┬──────┘
       │
       │ 1. Poll: GET /api/v1/products?lastUpdate={timestamp}
       │    (Every 5 minutes)
       │
┌──────▼──────────────────────────────────────┐
│   API Response:                              │
│   {                                          │
│     "hasUpdates": true,                      │
│     "updateTimestamp": "2024-01-15T10:30:00Z"│
│   }                                          │
└──────┬──────────────────────────────────────┘
       │
       │ 2. If hasUpdates=true:
       │    GET /api/v1/products/updates?since={timestamp}
       │
┌──────▼──────────────────────────────────────┐
│   Response: List of updated products         │
│   (Only changes, not entire catalog)        │
└──────────────────────────────────────────────┘
```

**Optimizations:**
- **Conditional Requests**: `If-Modified-Since` header
- **ETags**: Content validation without downloading
- **Delta Updates**: Only changes since last synchronization
- **Compression**: Gzip/Brotli on JSON responses

##### 4. Batching and Chunking

**For Large Downloads:**

```http
GET /api/v1/products/batch?ids=1,2,3,4,5
```

**Chunking for Large Lists:**
```http
GET /api/v1/products?page=1&pageSize=100
GET /api/v1/products?page=2&pageSize=100
```

**Advantages:**
- ✅ Load control per request
- ✅ Client-side parallelization
- ✅ Partial recovery in case of error

##### 5. Pre-fetching and Pre-caching

**Client Strategy:**
- **Pre-fetch**: Download likely needed content
- **Background Sync**: Background synchronization
- **Local Cache**: Store on device (IndexedDB, SQLite)

##### 6. Content-Specific Rate Limiting

```csharp
// Differentiated rate limiting
services.AddRateLimiter(options =>
{
    // Content endpoints: more permissive
    options.AddPolicy("content", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1000, // More permissive for content
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // API endpoints: more restrictive
    options.AddPolicy("api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

#### Solution Summary

| Strategy | Current Implementation | Recommendation |
|----------|----------------------|----------------|
| CDN | ⚠️ Not implemented | CloudFlare/CloudFront |
| Blob Storage | ⚠️ Local filesystem | Azure Blob / AWS S3 |
| Invalidation | ⚠️ Manual | Polling + ETags |
| Batching | ⚠️ Not implemented | Batch endpoints |
| Pre-fetching | ⚠️ Not implemented | Client strategy |
| Rate Limiting | ⚠️ Not implemented | Differentiated policies |

---

## 3. Large-Scale Data Queries

### Question
What mechanisms would you apply to optimize dashboards that query data from thousands of devices in real-time without affecting global performance?

### Answer

#### Optimized Dashboard Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Dashboard Frontend                    │
│              (Angular / React / Vue)                     │
└──────────────┬──────────────────────────────────────────┘
               │
               │ REST API (Real-time updates via polling)
               │
    ┌───────────▼──────────────────────────┐
    │      Dashboard API (Read-Only)        │
    │      (Separated from Main API)        │
    │                                       │
    │  - Optimized queries                  │
    │  - Aggressive caching                 │
    │  - Materialized Views                 │
    └───────────┬──────────────────────────┘
                │
    ┌───────────▼──────────────────────────┐
    │      Read-Only Database               │
    │      (Replica + Materialized Views)   │
    │                                       │
    │  - Pre-aggregations                   │
    │  - Optimized indexes                  │
    │  - Partitioning                      │
    └───────────────────────────────────────┘
```

#### Implemented and Recommended Strategies

##### 1. Separate Read Store for Dashboard

**CQRS Architecture Applied:**

```
┌─────────────────────────────────────────┐
│   Main API (Write + General Read)       │
│   - Commands: Writes                    │
│   - Queries: General queries            │
└──────────────┬──────────────────────────┘
               │
               │ Event Sourcing / CDC
               │
┌──────────────▼──────────────────────────┐
│   Dashboard Read Store (Optimized)     │
│   - Read-only                           │
│   - Pre-aggregations                    │
│   - Materialized Views                  │
└─────────────────────────────────────────┘
```

**CQRS Implementation:**
- ✅ **`Challenge.Queries` layer**: Implemented with queries for Categories, Products, and ProductCategories
- ✅ **MediatR**: Allows routing to different handlers
- ✅ **Query Handlers**: Use DbContext directly for optimized read operations
- ✅ **Pagination & Filtering**: Implemented in GetAll queries with support for sorting
- ⚠️ **Future**: Separate read store (SQL Server Read Replica or Azure SQL Data Warehouse)

**Advantages:**
- ✅ Load isolation: Dashboard doesn't affect critical operations
- ✅ Specific optimization: Indexes and materialized views only for dashboard
- ✅ Independent scaling: More replicas for dashboard if needed

##### 2. Materialized Views and Pre-aggregations

**Problem**: Complex queries over thousands of records in real-time

**Solution**: Pre-calculate aggregations

```sql
-- Example: Materialized view for dashboard
CREATE MATERIALIZED VIEW DashboardProductStats
WITH (DISTRIBUTION = HASH(ProductId))
AS
SELECT 
    p.Id AS ProductId,
    p.Name,
    COUNT(pc.CategoryId) AS CategoryCount,
    MAX(pc.CreatedDate) AS LastCategoryAssignment,
    p.Image
FROM Products p
LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId
GROUP BY p.Id, p.Name, p.Image;

-- Periodic update (every 5 minutes)
CREATE INDEX IX_DashboardProductStats_ProductId 
ON DashboardProductStats(ProductId);
```

**Update Strategy:**
- **Incremental**: Only recalculate changes since last update
- **Frequency**: Every 1-5 minutes (depending on "real-time" needs)
- **Background Job**: Azure Functions / Hangfire for updates

##### 3. Aggressive Caching for Dashboard

```
Request → CDN Cache (5 min) → Redis Cache (1 min) → Database
```

**Multi-Level Strategy:**

```csharp
// Recommended implementation
public class DashboardQueryHandler : IRequestHandler<GetDashboardDataQuery>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    
    public async Task<DashboardData> Handle(GetDashboardDataQuery request)
    {
        // Level 1: Memory Cache (ultra fast, per instance)
        var cacheKey = $"dashboard:{request.Filters}";
        if (_memoryCache.TryGetValue(cacheKey, out DashboardData cached))
            return cached;
        
        // Level 2: Distributed Cache (Redis, shared between instances)
        var distributed = await _distributedCache.GetStringAsync(cacheKey);
        if (distributed != null)
        {
            var data = JsonSerializer.Deserialize<DashboardData>(distributed);
            _memoryCache.Set(cacheKey, data, TimeSpan.FromMinutes(1));
            return data;
        }
        
        // Level 3: Database (only if not in cache)
        var data = await _repository.GetDashboardData(request.Filters);
        
        // Store in both cache levels
        await _distributedCache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(data), 
            new DistributedCacheEntryOptions 
            { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
            });
        _memoryCache.Set(cacheKey, data, TimeSpan.FromMinutes(1));
        
        return data;
    }
}
```

**Strategic TTL:**
- **Real-time data**: 30 seconds - 1 minute
- **Semi-static data**: 5-15 minutes
- **Historical data**: 1 hour or more

##### 4. Pagination and Lazy Loading

**Problem**: Loading thousands of records in dashboard

**Solution**: Efficient pagination

```csharp
// Query with optimized pagination
public class GetProductsQuery : IRequest<PaginatedResponse<ProductDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50; // Maximum reasonable
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
}

// Optimized handler
public async Task<PaginatedResponse<ProductDto>> Handle(GetProductsQuery request)
{
    var query = _context.Products.AsQueryable();
    
    // Filters
    if (!string.IsNullOrEmpty(request.SearchTerm))
        query = query.Where(p => p.Name.Contains(request.SearchTerm));
    
    // Sorting with index
    query = request.SortBy switch
    {
        "name" => query.OrderBy(p => p.Name),
        "name_desc" => query.OrderByDescending(p => p.Name),
        _ => query.OrderBy(p => p.Id)
    };
    
    // Total count (optimized with COUNT(*))
    var totalCount = await query.CountAsync();
    
    // Pagination (only bring current page)
    var items = await query
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .Select(p => new ProductDto 
        { 
            Id = p.Id, 
            Name = p.Name,
            // Only necessary fields (don't bring everything)
        })
        .ToListAsync();
    
    return new PaginatedResponse<ProductDto>
    {
        Items = items,
        TotalCount = totalCount,
        PageNumber = request.PageNumber,
        PageSize = request.PageSize
    };
}
```

**Virtual Scrolling**: In frontend, load only visible elements + buffer

##### 5. Optimized Indexes

```sql
-- Indexes for dashboard queries
CREATE NONCLUSTERED INDEX IX_Products_Name 
ON Products(Name) 
INCLUDE (Description, Image);

CREATE NONCLUSTERED INDEX IX_ProductCategories_ProductId_CategoryId
ON ProductCategories(ProductId, CategoryId)
INCLUDE (Id);

-- Composite index for frequent searches
CREATE NONCLUSTERED INDEX IX_Products_Search
ON Products(Name, Description)
WHERE Image IS NOT NULL;
```

##### 6. Dashboard-Specific Read Replicas

```
Primary DB (Write) ──┐
                     ├──> Replica 1 (General Read)
                     ├──> Replica 2 (Dashboard Read) ← Optimized
                     └──> Replica 3 (Analytics Read)
```

**Configuration:**
- **Dashboard Replica**: Maximum memory for query caching
- **Query Hints**: Force use of specific indexes
- **Connection String**: Direct routing to dashboard replica

#### Solution Summary

| Strategy | Current Implementation | Recommendation |
|----------|----------------------|----------------|
| Separate Read Store | ✅ CQRS implemented | Specific Read Replica |
| Materialized Views | ⚠️ Not implemented | Pre-aggregations |
| Multi-Level Cache | ⚠️ Not implemented | Memory + Redis |
| Pagination | ✅ Helpers available | Implement in queries |
| Optimized Indexes | ⚠️ Basic | Specific composite indexes |
