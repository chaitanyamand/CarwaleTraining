import { Heart } from "lucide-react";
import { useDispatch } from "react-redux";
import { CAR_LIKED, CAR_UNLIKED } from "../../redux/actions";
import "../../styles/CarCard.css";

export const CarCard = ({ car = {}, id, isLiked = false }) => {
  const dispatch = useDispatch();

  const imageUrl = car.imageUrl || (car.stockImages && car.stockImages[0]);

  let emiAmount = "N/A";
  try {
    const emiParts = car.emiText?.split(" ");
    if (emiParts && emiParts.length >= 4) {
      emiAmount = emiParts[3];
      if (emiParts[4]) emiAmount += emiParts[4];
    }
  } catch (e) {
    console.log(e);
    emiAmount = "N/A";
  }

  const handleImageError = (e) => {
    if (e.target) {
      e.target.onerror = null;
      e.target.style.display = "none";
      if (e.target.nextElementSibling) {
        e.target.nextElementSibling.style.display = "flex";
      }
    }
  };

  return (
    <div className="car-card">
      {car.isValueForMoney ? <div className="car-tag value-tag">Value For Money</div> : car.tagText ? <div className="car-tag">{car.tagText}</div> : null}

      <button
        onClick={() =>
          dispatch({
            type: isLiked ? CAR_UNLIKED : CAR_LIKED,
            payload: { id },
          })
        }
        className="heart-btn"
      >
        <Heart size={20} fill={isLiked ? "#ff4757" : "none"} color={isLiked ? "#ff4757" : "#666"} />
      </button>

      {imageUrl ? <img src={imageUrl} alt={`${car.makeYear || ""} ${car.carName || "Car"}`} onError={handleImageError} className="car-image" /> : null}

      <div className="image-fallback" style={{ display: imageUrl ? "none" : "flex" }}>
        No Image Available
      </div>

      <div className="car-info">
        <h3 className="car-title">{car.carName || "Unknown Model"}</h3>

        <div className="car-meta">
          <span>{car.km || "0"} km</span>
          <span>|</span>
          <span>{car.fuel || "Fuel Type"}</span>
          <span>|</span>
          <span>{car.cityName || "Unknown City"}</span>
        </div>

        <div className="car-price-offer">
          <div>
            <span className="car-price">{car.formattedPrice || "N/A"}</span>
            <button className="make-offer-btn">Make Offer</button>
          </div>
        </div>

        <div className="car-emi">
          EMI starts at <span>{emiAmount}</span>
        </div>

        <button className="view-seller-btn">Get Seller Details</button>
      </div>
    </div>
  );
};
