import { useDispatch, useSelector } from "react-redux";
import { CHANGE_MAX_PRICE, CHANGE_MIN_PRICE } from "../redux/actions";
import { Memoised } from "./Memoised";
import "../styles/BudgetFilter.css";

export const BudgetFilter = Memoised(() => {
  const dispatch = useDispatch();
  const minPrice = useSelector((state) => state.filter.priceRange[0]);
  const maxPrice = useSelector((state) => state.filter.priceRange[1]);

  return (
    <div className="budget-filter">
      <h4 className="budget-header">Budget</h4>

      <div className="slider-wrapper">
        <div className="slider-track"></div>

        <div
          className="slider-range"
          style={{
            left: `${(minPrice / 21) * 100}%`,
            width: `calc(${((maxPrice - minPrice) / 21) * 100}% - 1px)`,
          }}
        ></div>

        <input
          type="range"
          min="0"
          max="21"
          value={minPrice}
          onChange={(e) => {
            const value = parseInt(e.target.value);
            if (value <= maxPrice) {
              dispatch({ type: CHANGE_MIN_PRICE, payload: { price: value } });
            }
          }}
          className="range-input"
        />

        <input
          type="range"
          min="0"
          max="21"
          value={maxPrice}
          onChange={(e) => {
            const value = parseInt(e.target.value);
            if (value >= minPrice) {
              dispatch({ type: CHANGE_MAX_PRICE, payload: { price: value } });
            }
          }}
          className="range-input"
        />
      </div>

      <div className="slider-labels">
        <span>Any</span>
        <span>20+ Lakh</span>
      </div>

      <div className="number-inputs">
        <div className="input-group">
          <input
            type="number"
            value={minPrice}
            onChange={(e) => {
              const value = parseInt(e.target.value) || 0;
              if (value <= maxPrice) {
                dispatch({ type: CHANGE_MIN_PRICE, payload: { price: Math.max(0, value) } });
              }
            }}
          />
          <span className="unit-label">Lakh</span>
        </div>

        <span className="range-separator">-</span>

        <div className="input-group">
          <input
            type="number"
            value={maxPrice}
            onChange={(e) => {
              const value = parseInt(e.target.value) || 0;
              if (value >= minPrice) {
                dispatch({ type: CHANGE_MAX_PRICE, payload: { price: Math.min(21, value) } });
              }
            }}
          />
          <span className="unit-label">Lakh</span>
        </div>
      </div>
    </div>
  );
});
