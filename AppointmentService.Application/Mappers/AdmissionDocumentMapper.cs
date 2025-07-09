using AppointmentService.Domain.ReadModels;
using AppointmentService.Domain.WriteModels;

namespace AppointmentService.Application.Mappers;

public static class AdmissionDocumentMapper
{
    public static AdmissionDocumentCollection ToReadModel(AdmissionDocument document)
    {
        return new AdmissionDocumentCollection
        {
            DocumentId = document.DocumentId,
            UploaderId = document.UploaderId,
            Title = document.Title,
            FilePath = document.FilePath,
            Content = document.Content,
            CreatedAt = document.CreatedAt,
            CreatedBy = document.CreatedBy,
            IsActive = document.IsActive,
            UpdatedAt = document.UpdatedAt,
            UpdatedBy = document.UpdatedBy
        };
    }
}
