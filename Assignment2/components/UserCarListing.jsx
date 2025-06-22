import { useEffect } from "react";
import { FilterSidebar } from "./FilterSidebar";
import { CarsGrid } from "./CarsGrid";
import { Header } from "./Header";
import { useDispatch, useSelector } from "react-redux";
import { fetchProducts } from "../redux/thunk";
import { Memoised } from "./Memoised";
import "../styles/UsedCarsListing.css";

const UsedCarsListing = Memoised(() => {
  const dispatch = useDispatch();
  const totalNumbers = useSelector((state) => state.product.totalNumber);
  const cars = useSelector((state) => state.product.cars);

  useEffect(() => {
    console.log("Fetching products...");
    dispatch(fetchProducts());
  }, []);

  return (
    <div className="used-cars-container">
      <h2 className="used-cars-title">{totalNumbers} Used Cars In India</h2>

      <div className="used-cars-layout">
        <FilterSidebar />

        <div className="used-cars-content">
          <Header />
          <CarsGrid cars={cars} />
        </div>
      </div>
    </div>
  );
});

export default UsedCarsListing;
