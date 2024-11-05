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
            var unit = await _imageTypeRepository.GetAllPagedAsync(query =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    query = query.Where(v => v.Name.Contains(name));

                query = query.Where(e => !e.Deleted); // Assuming we filter out soft-deleted entities
                return query;

            }, pageIndex, pageSize);
            return unit;
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
        #endregion

        #region Utility
        // Add any additional utility methods here, if needed
        #endregion
    }
}
