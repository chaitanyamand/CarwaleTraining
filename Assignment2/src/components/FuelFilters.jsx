import { useSelector } from "react-redux";
import { FuelFilter } from "./FuelFilter";
import "../../styles/FuelFilters.css";

export const FuelFilters = () => {
  const fuelSelected = useSelector((state) => state.filter.fuelSelected);
  const fuelTypes = ["Petrol", "Diesel", "CNG", "LPG", "Electric", "Hybrid"];

  return (
    <div className="fuel-section">
      {fuelTypes.map((fuelType, index) => (
        <FuelFilter key={fuelType} fuelName={fuelType} noOfProfiles={0} index={index + 1} isChecked={fuelSelected.has(index + 1)} />
      ))}
    </div>
  );
};
