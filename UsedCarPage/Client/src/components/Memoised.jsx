import isEqual from "lodash.isequal";
import React from "react";

export const Memoised = (Component) => {
  return React.memo(Component, (prevProps, nextProps) => {
    return isEqual(prevProps, nextProps);
  });
};
