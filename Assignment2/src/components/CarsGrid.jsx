import { useSelector } from "react-redux";
import { CarCard } from "./CarCard";
import "../styles/CarsGrid.css";

export const CarsGrid = ({ cars }) => {
  const isLiked = useSelector((state) => state.product.likedCars);
  const status = useSelector((state) => state.product.status);

  if (status === "PENDING") {
    return (
      <div className="loading-container">
        <h2>Loading...</h2>
      </div>
    );
  }

  if (status === "FAILED") {
    return (
      <div className="error-container">
        <h2>Failed to load cars. Please try again later.</h2>
      </div>
    );
  }

  return (
    <div className="cars-grid-wrapper hide-scrollbar">
      <div className="cars-grid">
        {cars.map((car) => (
          <CarCard key={car.profileId} car={car} id={car.profileId} isLiked={isLiked.has(car.profileId)} />
        ))}
      </div>
    </div>
  );
};
