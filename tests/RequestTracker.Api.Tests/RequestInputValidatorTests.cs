using Xunit;
using RequestTracker.Api;

namespace RequestTracker.Api.Tests;

public sealed class RequestInputValidatorTests
{
    [Fact]
    public void Validate_ReturnsNoErrors_WhenAllInputsAreValid()
    {
        var errors = RequestInputValidator.Validate("Printer issue", "The printer is offline.", "Aye Aye");

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData(null, "A description", "Requester", "title")]
    [InlineData(" ", "A description", "Requester", "title")]
    [InlineData("A title", null, "Requester", "description")]
    [InlineData("A title", " ", "Requester", "description")]
    [InlineData("A title", "A description", " ", null)]
    public void Validate_ReportsMissingRequiredValues(string? title, string? description, string? requester, string? expectedField)
    {
        var errors = RequestInputValidator.Validate(title, description, requester);

        if (expectedField is null)
            Assert.Empty(errors);
        else
            Assert.Contains(expectedField, errors.Keys);
    }

    [Fact]
    public void Validate_RejectsTitleLongerThan120Characters()
    {
        var errors = RequestInputValidator.Validate(new string('x', 121), "A description", null);

        Assert.Contains("title", errors.Keys);
    }

    [Fact]
    public void Validate_RejectsDescriptionLongerThan2000Characters()
    {
        var errors = RequestInputValidator.Validate("A title", new string('x', 2001), null);

        Assert.Contains("description", errors.Keys);
    }
}
