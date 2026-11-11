namespace RequestTracker.Api;

public static class RequestInputValidator
{
    {
        var errors = new Dictionary<string, string[]>();
    
        if (string.IsNullOrWhiteSpace(title))
            errors["title"] = ["A title is required."];
        else if (title.Trim().Length > 120)
            errors["title"] = ["The title must be 120 characters or fewer."];
    
        if (string.IsNullOrWhiteSpace(description))
            errors["description"] = ["A description is required."];
        else if (description.Trim().Length > 2000)
            errors["description"] = ["The description must be 2000 characters or fewer."];
    
        if (requestedBy?.Trim().Length > 120)
            errors["requestedBy"] = ["Requested by must be 120 characters or fewer."];
    
        return errors;
    }
    
}
