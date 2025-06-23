import { FETCH_PRODUCT_FAILURE, FETCH_PRODUCT_PENDING, FETCH_PRODUCT_SUCCESS } from "./actions";

export const fetchProducts = () => {
    return (dispatch, getState) => {
        dispatch({ type: FETCH_PRODUCT_PENDING });

        const { filter } = getState();
        const fuelSelected = Array.from(filter.fuelSelected);
        const [minPrice, maxPrice] = filter.priceRange;

        const fuelParam = encodeURIComponent(fuelSelected.join("+"));
        const budgetParam = `${minPrice}-${maxPrice}`;

        const url = `http://localhost:5000/api/stocks?fuel=${fuelParam}&budget=${budgetParam}`;

        fetch(url)
            .then((res) => res.json())
            .then((data) => {
                const cars = data.stocks;
                console.log(cars);
                const totalNumber = data.totalCount;
                dispatch({ type: FETCH_PRODUCT_SUCCESS, payload: { data: cars, totalNumber } });
            })
            .catch((err) => {
                dispatch({ type: FETCH_PRODUCT_PENDING, error: err });
            });
    };
};
