import { useDispatch } from "react-redux";
import { Memoised } from "./Memoised";
import { CHANGE_MAX_PRICE, CHANGE_MIN_PRICE } from "../../redux/actions";
import "../../styles/PopularChoicesButton.css";

export const PopularChoiceButton = Memoised(({ name, min, max }) => {
  const dispatch = useDispatch();
  return (
    <button
      onClick={() => {
        dispatch({ type: CHANGE_MIN_PRICE, payload: { price: min } });
        dispatch({ type: CHANGE_MAX_PRICE, payload: { price: max } });
      }}
      className="popular-choice-btn"
    >
      {name}
    </button>
  );
});
