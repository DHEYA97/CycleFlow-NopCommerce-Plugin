using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.CycleFlow.Domain;

namespace Nop.Plugin.Misc.CycleFlow.Services
{
    public class ImageTypeService : IImageTypeService
    {
        #region Fields
        private readonly IRepository<ImageType> _imageTypeRepository;
        #endregion

        #region Ctor
        public ImageTypeService(IRepository<ImageType> imageTypeRepository)
        {
            _imageTypeRepository = imageTypeRepository;
        }
        #endregion

        #region Methods
        public async Task<IPagedList<ImageType>> GetAllImageTypesAsync(string name = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var image = await _imageTypeRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(e => !e.Deleted);
                
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                return query;

            }, pageIndex, pageSize);
            return image;
        }

        public async Task InsertImageTypeAsync(ImageType imageType)
        {
            await _imageTypeRepository.InsertAsync(imageType);
        }

        public async Task<ImageType> GetImageTypeByIdAsync(int imageTypeId)
        {
            return await _imageTypeRepository.GetByIdAsync(imageTypeId, cache => default);
        }

        public async Task UpdateImageTypeAsync(ImageType imageType)
        {
            await _imageTypeRepository.UpdateAsync(imageType);
        }

        public async Task DeleteImageTypeAsync(ImageType imageType)
        {
            await _imageTypeRepository.DeleteAsync(imageType);
        }

        public async Task<bool> IsImageTypeNameFoundAsync(string name, int id)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            name = name.Trim();
            return await _imageTypeRepository.Table.AnyAsync(t => t.Name == name && t.Id != id);
        }
        public virtual async Task<string> GetImageTypeNameAsync(int id)
        {
            return await _imageTypeRepository.Table
                           .Where(x => x.Id == id)
                           .Select(x => x.Name)
                           .FirstOrDefaultAsync() ?? string.Empty;
        }
        #endregion

        #region Utility
        #endregion
    }
}
