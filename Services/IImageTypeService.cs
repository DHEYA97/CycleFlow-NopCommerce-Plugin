using Nop.Core;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public interface IImageTypeService
    {
        Task<IPagedList<ImageType>> GetAllImageTypesAsync(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);
        Task InsertImageTypeAsync(ImageType imageType);
        Task<ImageType> GetImageTypeByIdAsync(int imageTypeId);
        Task UpdateImageTypeAsync(ImageType imageType);
        Task DeleteImageTypeAsync(ImageType imageType);
        Task<bool> IsImageTypeNameFoundAsync(string name, int id);
        Task<string> GetImageTypeNameAsync(int id);
    }
}
