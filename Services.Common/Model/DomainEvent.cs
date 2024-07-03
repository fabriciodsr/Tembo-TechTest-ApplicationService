using System.Globalization;

namespace Services.Common.Abstractions.Model;

public abstract record DomainEvent;

public record InvestorCreated(Guid UserId, string InvestorId) : DomainEvent;

public record AccountCreated(string InvestorId, ProductCode Product, string AccountId) : DomainEvent;

public record KycFailed(Guid UserId, Guid ReportId) : DomainEvent;

public record ApplicationCompleted(Guid ApplicationId) : DomainEvent;

public record KycCompleted(Guid UserId, Guid ReportId, bool IsVerified) : DomainEvent;

public record EligibilityCheckCompleted(Guid ApplicationId, ProductCode Product, bool IsEligible) : DomainEvent;

public record EligibilityCheckFailed(Guid ApplicationId, ProductCode Product, string Reason) : DomainEvent;

public record InvestorCreationFailed(Guid ApplicationId, ProductCode Product, string Reason) : DomainEvent;

public record AccountCreationFailed(Guid ApplicationId, string InvestorId, ProductCode Product, string Reason) : DomainEvent;

public record ApplicationFailed(Guid ApplicationId, ProductCode Product, string Reason) : DomainEvent;