import { FETCH_PRODUCT_FAILURE, FETCH_PRODUCT_SUCCESS, FETCH_PRODUCT_PENDING, CAR_LIKED, CAR_UNLIKED, SORT_BY_PRICE_LOW_TO_HIGH, SORT_BY_PRICE_HIGH_TO_LOW, SORT_BY_YEAR_NEWEST } from "./actions";

const initialState = {
    totalNumber: 0, cars: [], status: "PENDING", likedCars: new Set(), requestController: null, nextPageUrl: "", prevPageUrl: ""
}

export const productReducer = (state = initialState, action) => {
    switch (action.type) {
        case FETCH_PRODUCT_PENDING: {
            return { ...state, status: "PENDING", requestController : action.payload.requestController, nextPageUrl: "", prevPageUrl: ""};
        }
        case FETCH_PRODUCT_FAILURE: {
            return { ...state, status: "FAILED", requestController: null, nextPageUrl: "", prevPageUrl: ""};
        }
        case FETCH_PRODUCT_SUCCESS: {
            return {
                ...state,
                totalNumber: action.payload.totalNumber,
                cars: action.payload.data,
                status: "SUCCESS",
                requestController: null,
                nextPageUrl: action.payload.nextPageUrl,
                prevPageUrl: action.payload.prevPageUrl
            }
        }
        case CAR_LIKED: {
            const newSet = new Set(state.likedCars);
            newSet.add(action.payload.id);
            return { ...state, likedCars: newSet };
        }
        case CAR_UNLIKED: {
            const newSet = new Set(state.likedCars);
            newSet.delete(action.payload.id);
            return { ...state, likedCars: newSet };
        }
        case SORT_BY_PRICE_LOW_TO_HIGH: {
            const sorted = [...state.cars].sort(
                (a, b) => Number(a.price) - Number(b.price)
            );
            return { ...state, cars: sorted };
        }

        case SORT_BY_PRICE_HIGH_TO_LOW: {
            const sorted = [...state.cars].sort(
                (a, b) => Number(b.price) - Number(a.price)
            );
            return { ...state, cars: sorted };
        }

        case SORT_BY_YEAR_NEWEST: {
            const sorted = [...state.cars].sort((a, b) => b.makeYear - a.makeYear);
            return { ...state, cars: sorted };
        }
        default:
            return state;
    }
}