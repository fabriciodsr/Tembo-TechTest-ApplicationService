using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using static System.Net.Mime.MediaTypeNames;

public class KycService : IKycService
{
    public Task<Result<KycReport>> GetKycReportAsync(User user)
    {
        if (user == null)
        {
            return Task.FromResult(Result.Failure<KycReport>(new Error("KYCService", "InvalidUser", "User cannot be null")));
        }

        if (user.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Task.FromResult(Result.Failure<KycReport>(new Error("KYCService", "EligibilityFailed", "User is not eligible for KYC")));
        }

        var reportId = Guid.NewGuid();
        var kycReport = new KycReport(reportId, true);

        return Task.FromResult(Result.Success(kycReport));
    }
}