import { ADD_FILTER_FUEL_TYPE, REMOVE_FILTER_FUEL_TYPE, CHANGE_MIN_PRICE, CHANGE_MAX_PRICE, CLEAR_FILTERS } from "./actions";

const initialState = {
    fuelSelected: new Set(), priceRange: [0, 21],
}

export const filterReducer = (state = initialState, action) => {
    switch (action.type) {
        case ADD_FILTER_FUEL_TYPE: {
            const newSet = new Set(state.fuelSelected);
            newSet.add(action.payload.fuelIndex);
            return { ...state, fuelSelected: newSet };
        }

        case REMOVE_FILTER_FUEL_TYPE: {
            const newSet = new Set(state.fuelSelected);
            newSet.delete(action.payload.fuelIndex);
            return { ...state, fuelSelected: newSet };
        }

        case CHANGE_MAX_PRICE: {
            return { ...state, priceRange: [state.priceRange[0], action.payload.price] }
        }
        case CHANGE_MIN_PRICE: {
            return { ...state, priceRange: [action.payload.price, state.priceRange[1]] }
        }
        case CLEAR_FILTERS: {
            return { ...state, fuelSelected: new Set(), priceRange: [0, 21] }
        }
        default:
            return state;
    }

}