﻿using System;
using UnityEngine;

namespace KerbalSpaceProgram.Api
{
    internal static class CelestialBodyExtensions
    {
        [PublicAPI]
        public static Coordinates GetCoordinates([NotNull] this CelestialBody body, Vector3 location)
        {
            return new Coordinates(body.GetLatitude(location), body.GetLongitude(location));
        }

        /// <summary>
        /// Returns a unit vector perpendicular to the surface of the body at the given latitude and
        /// longitude (pretending for the moment that the body is a perfect sphere).
        /// </summary>
        [PublicAPI]
        public static Vector3d GetSurfaceNVector([NotNull] this CelestialBody body, Coordinates coordinates)
        {
            return body.GetSurfaceNVector(coordinates.Latitude, coordinates.Longitude);
        }

        /// <summary>
        /// Returns the altitude for the specified body and location in meters. Never returns a negative
        /// value.
        /// </summary>
        [PublicAPI]
        public static double GetTerrainAltitude([NotNull] this CelestialBody body, Coordinates coordinates)
        {
            if (body.pqsController == null) {
                // Sun
                return 0;
            }

            Vector3d pqsRadialVector = QuaternionD.AngleAxis(coordinates.Longitude, Vector3d.down) 
                                     * QuaternionD.AngleAxis(coordinates.Latitude, Vector3d.forward)
                                     * Vector3d.right;
            double ret = body.pqsController.GetSurfaceHeight(pqsRadialVector) - body.pqsController.radius;
            if (ret < 0) {
                ret = 0;
            }
            return ret;
        }

        [PublicAPI]
        public static Vector3d GetWorldSurfacePosition([NotNull] this CelestialBody body, Coordinates coordinates, double alt)
        {
            return body.GetWorldSurfacePosition(coordinates.Latitude, coordinates.Longitude, alt);
        }

        [PublicAPI]
        public static Coordinates? GetMouseCoordinates(this CelestialBody body)
        {
            Ray mouseRay = PlanetariumCamera.Camera.ScreenPointToRay(Input.mousePosition);
            mouseRay.origin = ScaledSpace.ScaledToLocalSpace(mouseRay.origin);
            Vector3d relOrigin = mouseRay.origin - body.position;
            double curRadius = body.pqsController.radiusMax;
            double lastRadius = 0;
            int loops = 0;

            while (loops < 50)
            {
                Vector3d relSurfacePosition;
                if (PQS.LineSphereIntersection(relOrigin, mouseRay.direction, curRadius, out relSurfacePosition))
                {
                    Vector3d surfacePoint = body.position + relSurfacePosition;
                    double alt = body.pqsController.GetSurfaceHeight(QuaternionD.AngleAxis(body.GetLongitude(surfacePoint), Vector3d.down) * QuaternionD.AngleAxis(body.GetLatitude(surfacePoint), Vector3d.forward) * Vector3d.right);
                    double error = Math.Abs(curRadius - alt);
                    if (error < (body.pqsController.radiusMax - body.pqsController.radiusMin) / 100)
                    {
                        return new Coordinates(body.GetLatitude(surfacePoint), AngleUtils.ClampDegrees180(body.GetLongitude(surfacePoint)));
                    }
                    else
                    {
                        lastRadius = curRadius;
                        curRadius = alt;
                        loops++;
                    }
                }
                else
                {
                    if (loops == 0)
                    {
                        break;
                    }
                    else
                    { // Went too low, needs to try higher
                        curRadius = (lastRadius * 9 + curRadius) / 10;
                        loops++;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tests if the body is occluding <paramref name="worldPosition"/> when viewed from
        /// <paramref name="cameraPosition"/>.
        /// </summary>
        [PublicAPI]
        public static bool IsOccluding([NotNull] this CelestialBody body, Vector3d worldPosition, Vector3 cameraPosition)
        {
            if (Vector3d.Distance(worldPosition, body.position) < body.Radius - 100)
            {
                return true;
            }

            if (Vector3d.Angle(cameraPosition - worldPosition, body.position - worldPosition) > 90)
            {
                return false;
            }

            double bodyDistance = Vector3d.Distance(cameraPosition, body.position);
            double separationAngle = Vector3d.Angle(worldPosition - cameraPosition, body.position - cameraPosition);
            double altitude = bodyDistance * Math.Sin(Math.PI / 180 * separationAngle);

            return altitude < body.Radius;
        }

        /// <summary>
        /// Tests if the body is occluding <paramref name="worldPosition"/> when viewed from
        /// the perspective of the <see cref="PlanetariumCamera"/>.
        /// </summary>
        [PublicAPI]
        public static bool IsOccluding([NotNull] this CelestialBody body, Vector3d worldPosition)
        {
            Vector3d camPos = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.Camera.transform.position);
            return body.IsOccluding(worldPosition, camPos);
        }
    }
}
