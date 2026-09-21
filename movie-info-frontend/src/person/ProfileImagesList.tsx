import { getTmdbImageUrl } from "../utilities/utilities";
import { type PersonProfileImage } from "./personTypes";

interface ProfileImagesListProps {
  name: string;
  images: PersonProfileImage[];
}

function ProfileImagesList({ name, images }: ProfileImagesListProps) {
  if (images.length === 0) {
    return <p className="detail-empty">None</p>;
  }

  return (
    <ul className="profile-images-list" data-testid="profile-images-list">
      {images.map((image, index) => (
        <li key={image.id}>
          {/* width / height only reserve the aspect ratio while the image
              lazy-loads; CSS caps the rendered width to the panel. */}
          <img
            src={getTmdbImageUrl(image.filePath, "w500") ?? undefined}
            alt={`${name} profile ${String(index + 1)}`}
            width={image.width}
            height={image.height}
            loading="lazy"
            className="profile-image"
          />
        </li>
      ))}
    </ul>
  );
}

export default ProfileImagesList;
