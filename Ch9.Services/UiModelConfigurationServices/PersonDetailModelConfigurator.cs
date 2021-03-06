﻿using Ch9.Services.Contracts;
using Ch9.Models;

using System.Collections.Generic;

namespace Ch9.Services.UiModelConfigurationServices
{

    public class PersonDetailModelConfigurator : IPersonDetailModelConfigurator
    {
        private readonly ISettings _settings;
        private TmdbConfigurationModel _tmdbConfiguration => _tmdbConfigurationCache.TmdbConfigurationModel;
        ITmdbConfigurationCache _tmdbConfigurationCache;

        private string ImageBaseUrl => _settings.UseHttpsForImages ? _tmdbConfiguration.Images.SecureBaseUrl : _tmdbConfiguration.Images.BaseUrl;

        public PersonDetailModelConfigurator(ISettings settings, ITmdbConfigurationCache tmdbConfigurationCache)
        {
            _settings = settings;
            _tmdbConfigurationCache = tmdbConfigurationCache;
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
        public void SetProfileGalleryPictureImageSrc(ImageModel profileGalleryImage, PersonsDetailsModel personModel, int profileImageSize = 1) =>
            profileGalleryImage.FilePath = ImageBaseUrl + _tmdbConfiguration.Images.ProfileSizes[profileImageSize] + personModel.ProfilePath;

        public void SetProfileGalleryImageSources(ImageModel[] profileImages, int profileImageSize = 1)
        {
            var urlPrefix = ImageBaseUrl + _tmdbConfiguration.Images.ProfileSizes[profileImageSize];

            foreach (var profileImage in profileImages)
                profileImage.FilePath = urlPrefix + profileImage.FilePath;
        }
    }
}
