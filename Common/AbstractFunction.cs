using System.Text.RegularExpressions;
using Common.Utils;
using Common.Utils.Const;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Common;

public class AbstractFunction<U, V> where U : AbstractResponse<V>
{

    /// <summary>
    /// Error check
    /// </summary>
    /// <param name="modelState"></param>
    /// <returns></returns>
    public static List<DetailError> ErrorCheck(ModelStateDictionary modelState)
    {
        var detailErrorList = new List<DetailError>();

        // If there is no error, return
        if (modelState.IsValid)
            return detailErrorList;

        foreach (var entry in modelState)
        {
            var key = entry.Key;
            var modelStateEntity = entry.Value;

            if (modelStateEntity == null || modelStateEntity.ValidationState == ModelValidationState.Valid)
                continue;

            // Remove the prefix "Value." from the key
            var keyReplace = Regex.Replace(key, @"^Value\.", "");
            keyReplace = Regex.Replace(keyReplace, @"^Value\[\d+\]\.", "");

            // Get error message
            var errorMessage = string.Join("; ", modelStateEntity.Errors.Select(e => e.ErrorMessage));

            var detailError = new DetailError();
            Match matchesKey;

            // Extract information from the key in the structure: object[index].property
            if ((matchesKey = new Regex(@"^(.*?)\[(\d+)\]\.(.*?)$").Match(keyReplace)).Success)
            {
                // In the case of a list
                detailError.field = matchesKey.Groups[1].Value;
            }
            else
            {
                // In the case of a single item
                detailError.field = keyReplace.Split('.').LastOrDefault();
            }

            // Convert the field name to lowercase
            detailError.field = StringUtil.ToLowerCase(detailError.field);

            // Set the error message
            detailError.MessageId = MessageId.E00000;
            detailError.ErrorMessage = errorMessage;

            detailErrorList.Add(detailError);
        }

        return detailErrorList;
    }
}