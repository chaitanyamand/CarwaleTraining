import { BudgetFilter } from "./BudgetFilter";
import { Funnel } from "lucide-react";
import { useDispatch } from "react-redux";
import { CLEAR_FILTERS } from "../../redux/actions";
import { Memoised } from "./Memoised";
import { PopularChoiceButton } from "./PopularChoiceButton";
import { FuelFilters } from "./FuelFilters";
import "../../styles/FilterSidebar.css";

export const FilterSidebar = Memoised(() => {
  const dispatch = useDispatch();

  const popularChoices = [
    { name: "Below 5L", min: 0, max: 5 },
    { name: "5L - 10L", min: 5, max: 10 },
    { name: "10L - 20L", min: 10, max: 20 },
  ];

  return (
    <div className="filter-sidebar">
      <div className="filter-header">
        <h3>
          <Funnel height={"20px"} width={"20px"} /> Filters
        </h3>
        <button className="clear-button" onClick={() => dispatch({ type: CLEAR_FILTERS })}>
          Clear All
        </button>
      </div>

      <FuelFilters />

      <BudgetFilter />

      <div className="popular-price-range">
        <h5>POPULAR PRICE RANGES</h5>
        <div className="popular-price-buttons">
          {popularChoices.map((popChoice) => (
            <PopularChoiceButton key={popChoice.name} {...popChoice} />
          ))}
        </div>
      </div>
    </div>
  );
});
