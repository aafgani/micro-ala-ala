window.paceOptions = {
  ajax: {
    trackMethods: ["GET", "POST", "PUT", "DELETE", "PATCH"], // track all relevant methods
    trackWebSockets: false,
    ignoreURLs: [],
  },
  restartOnRequestAfter: true, // restart pace if request takes time
  document: true, // track document loading
  eventLag: false, // disable event lag
};
