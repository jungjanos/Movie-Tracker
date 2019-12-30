using Ch9.Models;

using System.Collections.Generic;

namespace Ch9.Services.Contracts
{
    public interface IPersonDetailModelConfigurator
    {
        void SetProfileGalleryImageSources(ImageModel[] profileImages, int profileImageSize = 1);
        void SetProfileGalleryPictureImageSrc(ImageModel profileGalleryImage, PersonsDetailsModel personModel, int profileImageSize = 1);
        void SetProfileImageSrc(IEnumerable<IStaffMemberRole> staffMembers, int profileImageSize = 0);
    }
}
