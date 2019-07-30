using Ch9.ApiClient;
using Ch9.Models;
using System.Collections.Generic;

namespace Ch9.Utils
{
    public interface IPersonDetailModelConfigurator
    {
        void SetProfileGalleryPictureImageSrc(ImageModel profileGalleryImage, GetPersonsDetailsModel personModel, int profileImageSize = 1);
        void SetProfileImageSrc(IEnumerable<IStaffMemberRole> staffMembers, int profileImageSize = 0);
    }

    public class PersonDetailModelConfigurator : IPersonDetailModelConfigurator
    {
        private readonly ISettings _settings;
        private readonly TmdbConfigurationModel _tmdbConfiguration;

        private string ImageBaseUrl => _settings.UseHttpsForImages ? _tmdbConfiguration.Images.SecureBaseUrl : _tmdbConfiguration.Images.BaseUrl;

        public PersonDetailModelConfigurator(ISettings settings, TmdbConfigurationModel tmdbConfiguration)
        {
            _settings = settings;
            _tmdbConfiguration = tmdbConfiguration;
        }

        /// <summary>
        /// Sets the ProfileUrl property to the full url of the profile image hosted on the content server for the entire collection.
        /// </summary>        
        /// <param name="profileImageSize">Quality selector 0: lowest, 1: high, 2 very high, 3: original. Uses TmdbConfigurationModel </param>
        public void SetProfileImageSrc(IEnumerable<IStaffMemberRole> staffMembers, int profileImageSize = 0)
        {
            string baseUrl = ImageBaseUrl;

            foreach (var staff in staffMembers)
                staff.ProfileUrl = baseUrl + _tmdbConfiguration.Images.ProfileSizes[profileImageSize] + staff.ProfilePath;
        }

        /// <summary>
        /// Sets the FilePath property to the full url of the profile image hosted on the content server for the ImageModel. 
        /// The image url is composed from PersonDetailModel's relative image path, and the base path and settings extracted from TmdbConfigurationModel
        /// </summary>
        /// <param name="profileGalleryImage">image which needs its FilePath property set to the complete image file url</param>
        /// <param name="personModel">object from which the relative file url is copied</param>
        /// <param name="profileImageSize">Quality selector 0: lowest, 1: high, 2 very high, 3: original. Uses TmdbConfigurationModel</param>
        public void SetProfileGalleryPictureImageSrc(ImageModel profileGalleryImage, GetPersonsDetailsModel personModel, int profileImageSize = 1) =>
            profileGalleryImage.FilePath = ImageBaseUrl + _tmdbConfiguration.Images.ProfileSizes[profileImageSize] + personModel.ProfilePath;
    }
}
