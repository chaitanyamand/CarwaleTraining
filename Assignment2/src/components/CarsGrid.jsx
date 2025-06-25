import { useDispatch, useSelector } from "react-redux";
import { CarCard } from "./CarCard";
import "../../styles/CarsGrid.css";
import { fetchProducts } from "../../redux/thunk";
import { useEffect, useRef } from "react";

export const CarsGrid = ({ cars }) => {
  const isLiked = useSelector((state) => state.product.likedCars);
  const status = useSelector((state) => state.product.status);
  const nextPageUrl = useSelector((state) => state.product.nextPageUrl);
  const prevPageUrl = useSelector((state) => state.product.prevPageUrl);
  const topRef = useRef(null);
  const dispatch = useDispatch();

  useEffect(() => {
    if (topRef.current) {
      topRef.current.scrollTop = 0; // scroll to top
    }
  }, [cars]);

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
    <>
      <div ref={topRef}></div>
      <div className="cars-grid-wrapper hide-scrollbar">
        <div className="cars-grid">
          {cars.map((car) => (
            <CarCard key={car.profileId} car={car} id={car.profileId} isLiked={isLiked.has(car.profileId)} />
          ))}
        </div>
        <div className="pagination-controls">
          <div className="previous" style={{ visibility: prevPageUrl ? "visible" : "hidden" }} onClick={() => dispatch(fetchProducts(prevPageUrl))}>
            Previous
          </div>
          <div className="next" style={{ visibility: nextPageUrl ? "visible" : "hidden" }} onClick={() => dispatch(fetchProducts(nextPageUrl))}>
            Next
          </div>
        </div>
      </div>
    </>
  );
};
