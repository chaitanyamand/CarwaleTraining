import { useDispatch } from "react-redux";
import { ADD_FILTER_FUEL_TYPE, REMOVE_FILTER_FUEL_TYPE } from "../redux/actions";
import { Memoised } from "./Memoised";
import "../styles/FuelFilter.css";

export const FuelFilter = Memoised(({ fuelName, index, isChecked }) => {
  const dispatch = useDispatch();

  return (
    <label className="fuel-filter">
      <input
        type="checkbox"
        checked={isChecked}
        onChange={() => {
          dispatch({
            type: isChecked ? REMOVE_FILTER_FUEL_TYPE : ADD_FILTER_FUEL_TYPE,
            payload: { fuelIndex: index },
          });
        }}
        className="fuel-checkbox"
      />
      <span className="fuel-label">{fuelName}</span>
    </label>
  );
});
