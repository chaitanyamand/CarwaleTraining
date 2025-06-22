import { useDispatch } from "react-redux";
import { SORT_BY_PRICE_LOW_TO_HIGH, SORT_BY_PRICE_HIGH_TO_LOW, SORT_BY_YEAR_NEWEST } from "../redux/actions";
import { Memoised } from "./Memoised";
import "../styles/Header.css";

export const Header = Memoised(() => {
  const dispatch = useDispatch();

  const handleSortChange = (e) => {
    const value = e.target.value;
    if (value === "low-to-high") {
      dispatch({ type: SORT_BY_PRICE_LOW_TO_HIGH });
    } else if (value === "high-to-low") {
      dispatch({ type: SORT_BY_PRICE_HIGH_TO_LOW });
    } else if (value === "newest") {
      dispatch({ type: SORT_BY_YEAR_NEWEST });
    }
  };

  return (
    <div className="header-container">
      <div className="sort-controls">
        <span className="sort-label">Sort By:</span>
        <select onChange={handleSortChange} className="sort-select">
          <option value="low-to-high">Price: Low to High</option>
          <option value="high-to-low">Price: High to Low</option>
          <option value="newest">Year: Newest First</option>
        </select>
      </div>
    </div>
  );
});
