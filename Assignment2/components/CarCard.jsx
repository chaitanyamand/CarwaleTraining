import { Heart } from "lucide-react";
import { useDispatch } from "react-redux";
import { CAR_LIKED, CAR_UNLIKED } from "../redux/actions";
import "../styles/CarCard.css";

export const CarCard = ({ car, id, isLiked }) => {
  const dispatch = useDispatch();

  const imageUrl = car.imageUrl || car.stockImages?.[0];
  const emiAmount = car.emiText.split(" ")[3];

  return (
    <div className="car-card">
      {car.tagText && <div className="car-tag">{car.tagText}</div>}

      <button onClick={() => dispatch({ type: isLiked ? CAR_UNLIKED : CAR_LIKED, payload: { id } })} className="heart-btn">
        <Heart size={20} fill={isLiked ? "#ff4757" : "none"} color={isLiked ? "#ff4757" : "#666"} />
      </button>

      {imageUrl ? (
        <img
          src={imageUrl}
          alt={`${car.makeYear} ${car.carName}`}
          onError={(e) => {
            e.target.onerror = null;
            e.target.style.display = "none";
            e.target.nextElementSibling.style.display = "flex";
          }}
          className="car-image"
        />
      ) : null}

      <div className="image-fallback" style={{ display: imageUrl ? "none" : "flex" }}>
        No Image Available
      </div>

      <div className="car-info">
        <h3 className="car-title">
           {car.carName}
        </h3>

        <div className="car-meta">
          <span>{car.km} km</span>
          <span>|</span>
          <span>{car.fuel}</span>
          <span>|</span>
          <span>{car.cityName}</span>
        </div>

        <div className="car-price-offer">
          <div>
            <span className="car-price">Rs. {car.price}</span>
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
