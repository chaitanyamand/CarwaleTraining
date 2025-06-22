import { ADD_FILTER_FUEL_TYPE, REMOVE_FILTER_FUEL_TYPE, CHANGE_MIN_PRICE, CHANGE_MAX_PRICE, CLEAR_FILTERS } from "./actions";
import { fetchProducts } from "./thunk";

let timeoutId;

const debounceFetchMiddleware = ({ dispatch }) => (next) => (action) => {
    const result = next(action);

    const filterActions = [
        ADD_FILTER_FUEL_TYPE,
        REMOVE_FILTER_FUEL_TYPE,
        CHANGE_MIN_PRICE,
        CHANGE_MAX_PRICE,
        CLEAR_FILTERS
    ];

    if (filterActions.includes(action.type)) {
        if (timeoutId) {
            clearTimeout(timeoutId);
        }

        timeoutId = setTimeout(() => {
            dispatch(fetchProducts());
        }, 500);
    }

    return result;
};

export default debounceFetchMiddleware;
